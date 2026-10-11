using System.Threading.Tasks;

namespace CommunityToolkit.Mvvm
{
    /// <summary>
    /// 为具名区域提供视图导航能力。
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// 将区域导航到已注册视图。
        /// </summary>
        /// <param name="regionName">应显示目标视图的区域。</param>
        /// <param name="viewName">已注册的视图名称。</param>
        void Navigate(string regionName, string viewName);

        /// <summary>
        /// 将区域导航到已注册视图，并传递导航参数。
        /// </summary>
        /// <param name="regionName">应显示目标视图的区域。</param>
        /// <param name="viewName">已注册的视图名称。</param>
        /// <param name="parameter">通过 <see cref="NavigationContext.Parameter"/> 提供的参数。</param>
        void Navigate(string regionName, string viewName, Parameter parameter);

        /// <summary>
        /// 将区域导航到已注册视图，并在导航完成或被取消后返回结果。
        /// </summary>
        /// <param name="regionName">应显示目标视图的区域。</param>
        /// <param name="viewName">已注册的视图名称。</param>
        /// <param name="parameter">通过 <see cref="NavigationContext.Parameter"/> 提供的参数。</param>
        /// <returns>导航真正完成返回 <c>true</c>；被 <see cref="IConfirmNavigationRequest"/> 取消返回 <c>false</c>。</returns>
        Task<bool> NavigateAsync(string regionName, string viewName, Parameter parameter = null);

        /// <summary>
        /// 在存在上一条日志项时，将区域导航到上一条记录。
        /// </summary>
        /// <param name="regionName">需要后退导航日志的区域。</param>
        void GoBack(string regionName);

        /// <summary>
        /// 在存在下一条日志项时，将区域导航到下一条记录。
        /// </summary>
        /// <param name="regionName">需要前进导航日志的区域。</param>
        void GoForward(string regionName);

        /// <summary>
        /// 判断区域是否存在上一条导航日志项。
        /// </summary>
        /// <param name="regionName">需要检查导航日志的区域。</param>
        /// <returns>可以后退导航时返回 <c>true</c>；否则返回 <c>false</c>。</returns>
        bool CanGoBack(string regionName);

        /// <summary>
        /// 判断区域是否存在下一条导航日志项。
        /// </summary>
        /// <param name="regionName">需要检查导航日志的区域。</param>
        /// <returns>可以前进导航时返回 <c>true</c>；否则返回 <c>false</c>。</returns>
        bool CanGoForward(string regionName);

        /// <summary>
        /// 获取指定区域的导航日志。
        /// </summary>
        /// <param name="regionName">需要返回导航日志的区域。</param>
        /// <returns>与区域关联的导航日志。</returns>
        INavigationJournal GetJournal(string regionName);

        /// <summary>
        /// 解除区域注册并释放其所有页面、日志和容器引用。
        /// </summary>
        /// <param name="regionName">要释放的区域名称。</param>
        void RemoveRegion(string regionName);

    }
}
