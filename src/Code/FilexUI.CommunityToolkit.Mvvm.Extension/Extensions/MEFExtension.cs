using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace CommunityToolkit.Mvvm
{
    /// <summary>
    /// 轻量级模块系统的扩展方法。
    /// </summary>
    public static class MEFExtension
    {
        /// <summary>
        /// 启用模块发现和服务注册。
        /// </summary>
        /// <param name="services">用于构建应用程序服务提供程序的服务集合。</param>
        /// <returns>返回同一个服务集合，便于链式调用。</returns>
        /// <remarks>
        /// 未配置任何模块程序集或目录时，默认扫描当前应用程序基目录以及其下的 Modules 目录。
        /// 自动加载模块需要在根服务提供程序构建完成后调用 <see cref="UseMEF(IServiceProvider)"/> 触发。
        /// </remarks>
        public static IServiceCollection UseMEF(this IServiceCollection services)
        {
            return UseMEF(services, null);
        }

        /// <summary>
        /// 使用自定义发现选项启用模块发现和服务注册。
        /// </summary>
        /// <param name="services">用于构建应用程序服务提供程序的服务集合。</param>
        /// <param name="configure">可选回调，用于添加模块程序集或模块目录。</param>
        /// <returns>返回同一个服务集合，便于链式调用。</returns>
        /// <remarks>
        /// 所有已发现模块都会在服务提供程序构建前执行
        /// <see cref="IModule.RegisterServices(IServiceCollection)"/>。
        /// 只有元数据中启用 <see cref="ModuleAttribute.AutoLoad"/> 的模块会在
        /// <see cref="UseMEF(IServiceProvider)"/> 被调用时自动初始化。
        /// 未配置任何扫描来源时，默认扫描当前应用程序基目录以及其下的 Modules 目录。
        /// </remarks>
        public static IServiceCollection UseMEF(this IServiceCollection services, Action<MEFOptions> configure)
        {
            if (services == null)
            {
                throw new ArgumentNullException("services");
            }

            MEFService mefService = new MEFService();
            MEFOptions options = new MEFOptions();

            if (configure != null)
            {
                configure(options);
            }

            if (!options.Assemblies.Any() && !options.Directories.Any())
            {
                options.AddDirectory(AppDomain.CurrentDomain.BaseDirectory);
                options.AddDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules"));
            }

            mefService.DiscoverModules(options.Assemblies.ToArray());

            foreach (string directory in options.Directories)
            {
                mefService.DiscoverModules(directory, options.SearchPattern);
            }

            if (options.ModulesDiscovered != null)
            {
                options.ModulesDiscovered(mefService.DiscoveredModules);
            }

            mefService.PrepareAutoLoadModules();
            mefService.RegisterDiscoveredModules(services);

            services.AddSingleton<MEFService>(mefService);
            services.AddSingleton<IMEFService>(provider => provider.GetService<MEFService>());
            return services;
        }

        /// <summary>
        /// 在根服务提供程序构建完成后初始化自动加载模块。
        /// </summary>
        /// <param name="provider">根服务提供程序。</param>
        /// <returns>返回同一个服务提供程序，便于链式调用。</returns>
        /// <remarks>
        /// <see cref="IServiceCollection"/> 阶段还没有可用的 <see cref="IServiceProvider"/>，
        /// 因此模块的 <see cref="IModule.OnInitialized(IServiceProvider)"/> 需要在容器构建完成后执行。
        /// </remarks>
        public static IServiceProvider UseMEF(this IServiceProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            MEFService mefService = provider.GetService<MEFService>();
            if (mefService == null)
            {
                throw new InvalidOperationException("MEF has not been registered. Call UseMEF on IServiceCollection before building the service provider.");
            }

            mefService.InitializeAutoLoadModules(provider);
            return provider;
        }

        /// <summary>
        /// 直接注册一个模块，不经过程序集或目录发现。
        /// </summary>
        /// <typeparam name="T">要注册的模块类型。</typeparam>
        /// <param name="services">用于构建应用程序服务提供程序的服务集合。</param>
        /// <returns>返回同一个服务集合，便于链式调用。</returns>
        /// <remarks>
        /// 该方法会立即执行模块的服务注册，并注册模块实例，使其能够参与自动初始化。
        /// </remarks>
        public static IServiceCollection AddModule<T>(this IServiceCollection services) where T : class, IModule
        {
            if (services == null)
            {
                throw new ArgumentNullException("services");
            }

            IModule module = MEFService.CreateAndRegisterModuleServices(services, typeof(T));

            if (module != null && !services.Any(item => item.ServiceType == typeof(T) && ReferenceEquals(item.ImplementationInstance, module)))
            {
                services.AddSingleton(typeof(T), module);
            }
            else if (module == null && !services.Any(item => item.ServiceType == typeof(T) && item.ImplementationType == typeof(T)))
            {
                services.AddSingleton<T>();
            }

            if (module != null && !services.Any(item => item.ServiceType == typeof(IModule) && ReferenceEquals(item.ImplementationInstance, module)))
            {
                services.AddSingleton(typeof(IModule), module);
            }
            else if (module == null && !services.Any(item => item.ServiceType == typeof(IModule) && item.ImplementationType == typeof(T)))
            {
                services.AddSingleton<IModule>(provider => provider.GetService<T>());
            }

            return services;
        }
    }
}
