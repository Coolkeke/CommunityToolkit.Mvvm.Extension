using System;

namespace CommunityToolkit.Mvvm
{
    /// <summary>
    /// <see cref="IModuleInfo"/> 的默认实现。
    /// </summary>
    public class ModuleInfo : IModuleInfo
    {
        /// <summary>
        /// 根据已发现模块类型初始化模块元数据，不创建模块实例。
        /// </summary>
        /// <param name="moduleType">具体模块类型。</param>
        public ModuleInfo(Type moduleType)
        {
            if (moduleType == null)
            {
                throw new ArgumentNullException("moduleType");
            }

            ModuleType = moduleType;
            ModuleAttribute attribute = GetModuleAttribute(moduleType);
            Name = string.IsNullOrWhiteSpace(attribute == null ? null : attribute.Name) ? moduleType.Name : attribute.Name;
            Description = attribute == null || string.IsNullOrWhiteSpace(attribute.Description) ? string.Empty : attribute.Description;
            AutoLoad = attribute == null || attribute.AutoLoad;
        }

        /// <summary>
        /// 根据已创建的模块实例初始化模块元数据。
        /// </summary>
        /// <param name="module">模块实例。</param>
        public ModuleInfo(IModule module)
        {
            if (module == null)
            {
                throw new ArgumentNullException("module");
            }

            ModuleType = module.GetType();
            ModuleAttribute attribute = GetModuleAttribute(ModuleType);
            Name = string.IsNullOrWhiteSpace(attribute == null ? null : attribute.Name) ? ModuleType.Name : attribute.Name;
            Description = attribute == null || string.IsNullOrWhiteSpace(attribute.Description) ? string.Empty : attribute.Description;
            AutoLoad = attribute == null || attribute.AutoLoad;
            Instance = module;
        }

        /// <summary>
        /// 获取模块显示名称。
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 获取模块描述。
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// 获取一个值，指示模块是否自动初始化。
        /// </summary>
        public bool AutoLoad { get; private set; }

        /// <summary>
        /// 获取具体模块类型。
        /// </summary>
        public Type ModuleType { get; private set; }

        /// <summary>
        /// 在可用时获取模块实例。
        /// </summary>
        public IModule Instance { get; private set; }

        private static ModuleAttribute GetModuleAttribute(Type moduleType)
        {
            return Attribute.GetCustomAttribute(moduleType, typeof(ModuleAttribute), false) as ModuleAttribute;
        }
    }
}
