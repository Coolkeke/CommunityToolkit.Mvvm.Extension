namespace CommunityToolkit.Mvvm
{
    /// <summary>
    /// 定义导航页面在失去活动状态后的保留策略。
    /// </summary>
    /// <remarks>
    /// 此接口的语义与 Prism 的 <c>IRegionMemberLifetime</c> 一致：未实现该接口时，
    /// 页面默认保留；返回 <c>false</c> 时，页面离开区域后会被移除。
    /// </remarks>
    public interface INavigationLifetime
    {
        /// <summary>
        /// 获取一个值，指示页面失去活动状态后是否仍保留在区域中。
        /// </summary>
        bool KeepAlive { get; }
    }
}
