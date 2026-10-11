using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace CommunityToolkit.Mvvm
{
    /// <summary>
    /// 为 <see cref="MEFExtension.UseMEF(IServiceCollection, Action{MEFOptions})"/> 配置模块发现行为。
    /// </summary>
    public sealed class MEFOptions
    {
        private readonly List<Assembly> assemblies = new List<Assembly>();
        private readonly List<string> directories = new List<string>();

        /// <summary>
        /// 初始化一个新的选项实例，并将默认目录搜索模式设置为 <c>*.dll</c>。
        /// </summary>
        public MEFOptions()
        {
            SearchPattern = "*.dll";
        }

        /// <summary>
        /// 获取或设置扫描目录中的模块程序集时使用的文件匹配模式。
        /// </summary>
        public string SearchPattern { get; set; }

        internal Action<IReadOnlyList<IModuleInfo>> ModulesDiscovered { get; private set; }

        internal IEnumerable<Assembly> Assemblies
        {
            get { return assemblies; }
        }

        internal IEnumerable<string> Directories
        {
            get { return directories; }
        }

        /// <summary>
        /// 添加一个需要扫描模块类型的程序集。
        /// </summary>
        /// <param name="assembly">包含一个或多个 <see cref="IModule"/> 实现的程序集。</param>
        /// <returns>当前选项实例。</returns>
        public MEFOptions AddModule(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException("assembly");
            }

            if (!assemblies.Contains(assembly))
            {
                assemblies.Add(assembly);
            }

            return this;
        }

        /// <summary>
        /// 添加包含指定模块类型的程序集。
        /// </summary>
        /// <param name="moduleType">实现 <see cref="IModule"/> 的具体类型。</param>
        /// <returns>当前选项实例。</returns>
        public MEFOptions AddModule(Type moduleType)
        {
            if (moduleType == null)
            {
                throw new ArgumentNullException("moduleType");
            }

            if (!typeof(IModule).IsAssignableFrom(moduleType))
            {
                throw new ArgumentException("Module type must implement IModule.", "moduleType");
            }

            return AddModule(moduleType.Assembly);
        }

        /// <summary>
        /// 添加包含指定模块类型的程序集。
        /// </summary>
        /// <typeparam name="T">用于定位待扫描程序集的具体模块类型。</typeparam>
        /// <returns>当前选项实例。</returns>
        public MEFOptions AddModule<T>() where T : class, IModule
        {
            return AddModule(typeof(T));
        }

        /// <summary>
        /// 添加一个需要扫描模块程序集的目录。
        /// </summary>
        /// <param name="directory">要扫描的目录路径。</param>
        /// <returns>当前选项实例。</returns>
        public MEFOptions AddDirectory(string directory)
        {
            if (string.IsNullOrWhiteSpace(directory))
            {
                throw new ArgumentException("Directory cannot be empty.", "directory");
            }

            string fullPath = Path.IsPathRooted(directory)
                ? Path.GetFullPath(directory)
                : Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, directory));

            if (!directories.Contains(fullPath))
            {
                directories.Add(fullPath);
            }

            return this;
        }

        /// <summary>
        /// 注册一个回调，在模块发现完成且服务注册开始前执行。
        /// </summary>
        /// <param name="action">接收所有已发现模块元数据的回调。</param>
        /// <returns>当前选项实例。</returns>
        public MEFOptions OnModulesDiscovered(Action<IReadOnlyList<IModuleInfo>> action)
        {
            ModulesDiscovered = action;
            return this;
        }
    }
}
