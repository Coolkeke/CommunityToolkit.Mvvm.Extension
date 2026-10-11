namespace CommunityToolkit.Mvvm
{
    /// <summary>
    /// 定义导航页面实例的创建和销毁生命周期。
    /// </summary>
    /// <remarks>
    /// <see cref="OnInitialized(NavigationContext)"/> 仅在视图和其数据上下文首次创建后调用。
    /// 历史后退和前进会复用已有实例，不会再次调用。<see cref="OnDestroyed"/> 仅在视图
    /// 从区域移除时调用，而不是在页面失去活动状态时调用。
    /// </remarks>
    public interface INavigationLifecycle
    {
        /// <summary>
        /// 在页面实例首次完成导航后调用。
        /// </summary>
        /// <param name="context">首次导航的上下文。</param>
        void OnInitialized(NavigationContext context);

        /// <summary>
        /// 在页面实例从区域移除、不会再复用时调用。
        /// </summary>
        void OnDestroyed();
    }
}
