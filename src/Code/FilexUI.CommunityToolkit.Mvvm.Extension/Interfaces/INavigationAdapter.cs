using System;

namespace CommunityToolkit.Mvvm
{
    /// <summary>
    /// 以非泛型方式描述可处理的区域目标，供导航服务在运行时选择适配器。
    /// </summary>
    public interface INavigationAdapter
    {
        /// <summary>
        /// 获取此适配器可以处理的区域目标类型。
        /// </summary>
        Type RegionTargetType { get; }

        /// <summary>
        /// 基于指定区域目标初始化区域。
        /// </summary>
        IRegion Initialize(object regionTarget, string regionName);
    }

    /// <summary>
    /// 适配 UI 控件，使其可以承载具名导航区域。
    /// </summary>
    /// <typeparam name="T">可被适配为区域的控件类型。</typeparam>
    public interface INavigationAdapter<T>
    {
        /// <summary>
        /// 基于指定 UI 控件初始化区域。
        /// </summary>
        /// <param name="regionTarget">用于显示区域视图的控件。</param>
        /// <param name="regionName">分配给区域的名称。</param>
        /// <returns>初始化后的区域实例。</returns>
        IRegion Initialize(T regionTarget, string regionName);
    }
}
