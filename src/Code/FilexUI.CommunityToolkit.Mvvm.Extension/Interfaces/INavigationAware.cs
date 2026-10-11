namespace CommunityToolkit.Mvvm
{
    /// <summary>
    /// 定义视图或其数据上下文的导航生命周期回调。
    /// </summary>
    public interface INavigationAware
    {
        /// <summary>
        /// 判断当前实例是否可复用于本次导航请求。
        /// </summary>
        /// <param name="context">本次导航请求的上下文。</param>
        /// <returns>复用现有视图实例时返回 <c>true</c>；否则返回 <c>false</c>。</returns>
        bool IsNavigationTarget(NavigationContext context);

        /// <summary>
        /// 在视图成为区域中的活动视图后调用。
        /// </summary>
        /// <param name="context">已完成导航的上下文。</param>
        void OnNavigatedTo(NavigationContext context);

        /// <summary>
        /// 在导航离开当前视图前调用。
        /// </summary>
        /// <param name="context">即将离开的导航上下文。</param>
        void OnNavigatedFrom(NavigationContext context);
    }
}
