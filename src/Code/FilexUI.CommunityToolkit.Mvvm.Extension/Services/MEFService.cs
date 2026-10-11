using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace CommunityToolkit.Mvvm
{
    /// <summary>
    /// <see cref="MEFExtension.UseMEF(IServiceCollection, Action{MEFOptions})"/> 使用的默认模块管理器。
    /// </summary>
    /// <remarks>
    /// 该服务将模块发现、服务注册和模块初始化拆开处理，因此标记为
    /// <see cref="ModuleAttribute.AutoLoad"/> 等于 <c>false</c> 的模块，
    /// 可以在启动阶段注册依赖，但延迟到调用 <see cref="LoadModule(string)"/>
    /// 或其他加载重载时再激活。
    /// </remarks>
    public class MEFService : IMEFService
    {
        private readonly List<IModuleInfo> discoveredModules = new List<IModuleInfo>();
        private readonly List<IModuleInfo> autoLoadModules = new List<IModuleInfo>();
        private readonly List<IModuleInfo> modules = new List<IModuleInfo>();
        private readonly Dictionary<Type, IModule> registeredModuleInstances = new Dictionary<Type, IModule>();
        private IServiceProvider serviceProvider;
        private bool initialized;

        /// <summary>
        /// 在模块完成初始化后发生。
        /// </summary>
        public event Action<IModuleInfo> ModuleLoaded;

        /// <summary>
        /// 获取从已配置程序集和目录中发现的所有模块类型。
        /// </summary>
        public IReadOnlyList<IModuleInfo> DiscoveredModules
        {
            get { return discoveredModules.AsReadOnly(); }
        }

        /// <summary>
        /// 获取将被自动初始化的已发现模块。
        /// </summary>
        public IReadOnlyList<IModuleInfo> AutoLoadModules
        {
            get { return autoLoadModules.AsReadOnly(); }
        }

        /// <summary>
        /// 获取已经完成初始化的模块。
        /// </summary>
        public IReadOnlyList<IModuleInfo> Modules
        {
            get { return modules.AsReadOnly(); }
        }

        /// <summary>
        /// 从程序集中发现具体的 <see cref="IModule"/> 实现。
        /// </summary>
        /// <param name="assemblies">要扫描的程序集。</param>
        public void DiscoverModules(params Assembly[] assemblies)
        {
            if (assemblies == null)
            {
                return;
            }

            foreach (Assembly assembly in assemblies.Where(item => item != null))
            {
                foreach (Type type in GetLoadableTypes(assembly))
                {
                    if (!type.IsClass || type.IsAbstract)
                    {
                        continue;
                    }

                    if (typeof(IModule).IsAssignableFrom(type))
                    {
                        AddDiscoveredModule(type);
                    }
                }
            }
        }

        /// <summary>
        /// 使用默认 <c>*.dll</c> 搜索模式从目录中发现模块程序集。
        /// </summary>
        /// <param name="directory">包含模块程序集的目录。</param>
        public void DiscoverModules(string directory)
        {
            DiscoverModules(directory, "*.dll");
        }

        /// <summary>
        /// 使用自定义文件搜索模式从目录中发现模块程序集。
        /// </summary>
        /// <param name="directory">包含模块程序集的目录。</param>
        /// <param name="searchPattern">用于定位程序集的文件匹配模式。</param>
        public void DiscoverModules(string directory, string searchPattern)
        {
            if (string.IsNullOrWhiteSpace(directory) || !Directory.Exists(directory))
            {
                return;
            }

            List<Assembly> assemblies = new List<Assembly>();
            foreach (string file in Directory.GetFiles(directory, string.IsNullOrWhiteSpace(searchPattern) ? "*.dll" : searchPattern))
            {
                try
                {
                    assemblies.Add(Assembly.LoadFrom(file));
                }
                catch
                {
                    // 忽略不是有效托管程序集的文件。
                }
            }

            DiscoverModules(assemblies.ToArray());
        }

        /// <summary>
        /// 准备应自动初始化的模块列表。
        /// </summary>
        internal void PrepareAutoLoadModules()
        {
            autoLoadModules.Clear();

            foreach (IModuleInfo moduleInfo in discoveredModules)
            {
                if (ShouldAutoLoad(moduleInfo))
                {
                    autoLoadModules.Add(moduleInfo);
                }
            }
        }

        /// <summary>
        /// 按具体类型初始化已发现模块。
        /// </summary>
        /// <param name="moduleType">要初始化的具体模块类型。</param>
        /// <returns>模块完成初始化时返回 <c>true</c>；否则返回 <c>false</c>。</returns>
        public bool LoadModule(Type moduleType)
        {
            if (moduleType == null)
            {
                throw new ArgumentNullException("moduleType");
            }

            if (!typeof(IModule).IsAssignableFrom(moduleType))
            {
                throw new ArgumentException("Module type must implement IModule.", "moduleType");
            }

            IModuleInfo moduleInfo = discoveredModules.FirstOrDefault(item => item.ModuleType == moduleType);
            if (moduleInfo == null)
            {
                return false;
            }

            return LoadModule(moduleInfo);
        }

        /// <summary>
        /// 按泛型类型初始化已发现模块。
        /// </summary>
        /// <typeparam name="T">要初始化的具体模块类型。</typeparam>
        /// <returns>模块完成初始化时返回 <c>true</c>；否则返回 <c>false</c>。</returns>
        public bool LoadModule<T>() where T : class, IModule
        {
            return LoadModule(typeof(T));
        }

        /// <summary>
        /// 按模块名称初始化已发现模块。
        /// </summary>
        /// <param name="moduleName">模块元数据名称或模块类型名称。</param>
        /// <returns>模块完成初始化时返回 <c>true</c>；否则返回 <c>false</c>。</returns>
        public bool LoadModule(string moduleName)
        {
            if (string.IsNullOrWhiteSpace(moduleName))
            {
                throw new ArgumentException("Module name cannot be empty.", "moduleName");
            }

            IModuleInfo moduleInfo = discoveredModules.FirstOrDefault(item => string.Equals(item.Name, moduleName, StringComparison.OrdinalIgnoreCase));
            if (moduleInfo == null)
            {
                return false;
            }

            return LoadModule(moduleInfo);
        }

        private bool LoadModule(IModuleInfo moduleInfo)
        {
            if (modules.Any(item => item.ModuleType == moduleInfo.ModuleType))
            {
                return false;
            }

            if (serviceProvider == null)
            {
                return false;
            }

            // 优先复用启动阶段创建的模块实例，确保 RegisterServices 和 OnInitialized 作用于同一个对象。
            IModule module;
            if (!registeredModuleInstances.TryGetValue(moduleInfo.ModuleType, out module))
            {
                module = CreateModule(serviceProvider, moduleInfo.ModuleType);
                if (module != null)
                {
                    registeredModuleInstances.Add(moduleInfo.ModuleType, module);
                }
            }

            if (module == null)
            {
                return false;
            }

            module.OnInitialized(serviceProvider);
            AddLoadedModule(module);
            return true;
        }

        private static bool ShouldAutoLoad(IModuleInfo moduleInfo)
        {
            if (moduleInfo == null)
            {
                return false;
            }

            return moduleInfo.AutoLoad;
        }

        /// <summary>
        /// 为所有已发现模块执行服务注册。
        /// </summary>
        /// <param name="services">用于构建根服务提供程序的服务集合。</param>
        internal void RegisterDiscoveredModules(IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException("services");
            }

            foreach (IModuleInfo moduleInfo in discoveredModules)
            {
                // 每个已发现模块都可以在根服务提供程序构建前注册服务。
                IModule module = CreateAndRegisterModuleServices(services, moduleInfo);
                // 自动加载模块也会注册为 IModule，便于启动初始化阶段解析。
                if (module != null && autoLoadModules.Any(item => item.ModuleType == moduleInfo.ModuleType))
                {
                    AddModuleRegistration(services, new ModuleInfo(module));
                }

                if (module != null && !registeredModuleInstances.ContainsKey(moduleInfo.ModuleType))
                {
                    registeredModuleInstances.Add(moduleInfo.ModuleType, module);
                }
            }
        }

        /// <summary>
        /// 初始化所有已准备自动加载的模块。
        /// </summary>
        /// <param name="provider">根服务提供程序。</param>
        internal void InitializeAutoLoadModules(IServiceProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            serviceProvider = provider;

            if (initialized)
            {
                return;
            }

            initialized = true;

            foreach (IModuleInfo moduleInfo in autoLoadModules)
            {
                LoadModule(moduleInfo);
            }
        }

        private void AddLoadedModule(IModule module)
        {
            if (modules.Any(item => item.ModuleType == module.GetType()))
            {
                return;
            }

            ModuleInfo moduleInfo = new ModuleInfo(module);
            modules.Add(moduleInfo);

            Action<IModuleInfo> moduleLoaded = ModuleLoaded;
            if (moduleLoaded != null)
            {
                moduleLoaded(moduleInfo);
            }

        }

        private void AddDiscoveredModule(Type moduleType)
        {
            if (discoveredModules.Any(item => item.ModuleType == moduleType))
            {
                return;
            }

            discoveredModules.Add(CreateModuleInfo(moduleType));
        }

        private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types.Where(type => type != null);
            }
        }

        /// <summary>
        /// 创建模块，并让它向指定服务集合注册服务。
        /// </summary>
        /// <param name="services">接收模块服务注册的服务集合。</param>
        /// <param name="moduleType">具体模块类型。</param>
        /// <returns>类型可被构造时返回创建出的模块实例；否则返回 <c>null</c>。</returns>
        internal static IModule CreateAndRegisterModuleServices(IServiceCollection services, Type moduleType)
        {
            return CreateAndRegisterModuleServices(services, CreateModuleInfo(moduleType));
        }

        private static IModule CreateAndRegisterModuleServices(IServiceCollection services, IModuleInfo moduleInfo)
        {
            IModule module = moduleInfo.Instance ?? CreateModule(moduleInfo.ModuleType);
            if (module != null)
            {
                module.RegisterServices(services);
            }

            return module;
        }

        private static void AddModuleRegistration(IServiceCollection services, IModuleInfo moduleInfo)
        {
            AddConcreteModuleRegistration(services, moduleInfo);

            Type moduleType = moduleInfo.ModuleType;
            if (moduleInfo.Instance != null)
            {
                if (!services.Any(item => item.ServiceType == typeof(IModule) && ReferenceEquals(item.ImplementationInstance, moduleInfo.Instance)))
                {
                    services.AddSingleton(typeof(IModule), moduleInfo.Instance);
                }

                return;
            }

            if (!services.Any(item => item.ServiceType == typeof(IModule) && item.ImplementationType == moduleType))
            {
                services.AddSingleton(typeof(IModule), provider => provider.GetService(moduleType));
            }
        }

        private static void AddConcreteModuleRegistration(IServiceCollection services, IModuleInfo moduleInfo)
        {
            Type moduleType = moduleInfo.ModuleType;
            if (moduleInfo.Instance == null)
            {
                if (!services.Any(item => item.ServiceType == moduleType && item.ImplementationType == moduleType))
                {
                    services.AddSingleton(moduleType, moduleType);
                }
            }
            else
            {
                if (!services.Any(item => item.ServiceType == moduleType && ReferenceEquals(item.ImplementationInstance, moduleInfo.Instance)))
                {
                    services.AddSingleton(moduleType, moduleInfo.Instance);
                }
            }
        }

        private static IModuleInfo CreateModuleInfo(Type moduleType)
        {
            return new ModuleInfo(moduleType);
        }

        private static IModule CreateModule(Type moduleType)
        {
            ConstructorInfo constructor = moduleType.GetConstructor(Type.EmptyTypes);
            if (constructor == null)
            {
                return null;
            }

            return Activator.CreateInstance(moduleType) as IModule;
        }

        private static IModule CreateModule(IServiceProvider provider, Type moduleType)
        {
            return ActivatorUtilities.CreateInstance(provider, moduleType) as IModule;
        }
    }
}
