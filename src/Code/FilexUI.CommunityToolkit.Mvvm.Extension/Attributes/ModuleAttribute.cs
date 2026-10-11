using System;

namespace CommunityToolkit.Mvvm
{
    /// <summary>
    /// 将类标记为应用程序模块，并提供模块元数据。
    /// </summary>
    /// <remarks>
    /// 被标记的类必须实现 <see cref="IModule"/>，才能被模块系统发现。
    /// <see cref="AutoLoad"/> 用于控制是否在启动阶段自动调用
    /// <see cref="IModule.OnInitialized(IServiceProvider)"/>。无论是否自动加载，
    /// 已发现模块的服务注册都会在根服务提供程序构建之前执行。
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ModuleAttribute : Attribute
    {
        /// <summary>
        /// 获取或设置模块显示名称。未指定时使用模块类型名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 获取或设置便于阅读的模块描述文本。
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 获取或设置一个值，指示模块是否应自动初始化。
        /// </summary>
        /// <remarks>
        /// 当该值为 <c>false</c> 时，启动阶段仍会调用 <see cref="IModule.RegisterServices"/>，
        /// 但 <see cref="IModule.OnInitialized"/> 会延迟到通过
        /// <see cref="IMEFService.LoadModule(string)"/> 或相关重载手动加载模块时执行。
        /// </remarks>
        public bool AutoLoad { get; set; }

        /// <summary>
        /// 初始化一个新的模块特性，并默认启用自动初始化。
        /// </summary>
        public ModuleAttribute()
        {
            AutoLoad = true;
        }
    }
}
