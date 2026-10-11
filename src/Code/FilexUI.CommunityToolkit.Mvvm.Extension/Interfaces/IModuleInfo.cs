using System;

namespace CommunityToolkit.Mvvm
{
    /// <summary>
    /// 描述一个已发现模块，使调用方无需直接读取反射元数据。
    /// </summary>
    public interface IModuleInfo
    {
        /// <summary>
        /// 获取 <see cref="ModuleAttribute.Name"/> 声明的显示名称；未指定名称时使用模块类型名称。
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 获取 <see cref="ModuleAttribute.Description"/> 声明的可选描述。
        /// </summary>
        string Description { get; }

        /// <summary>
        /// 获取一个值，指示模块是否应在应用程序启动期间自动初始化。
        /// </summary>
        bool AutoLoad { get; }

        /// <summary>
        /// 获取已发现的具体模块类型。
        /// </summary>
        Type ModuleType { get; }

        /// <summary>
        /// 模块已经创建时获取其实例；否则为 <c>null</c>。
        /// </summary>
        IModule Instance { get; }
    }
}
