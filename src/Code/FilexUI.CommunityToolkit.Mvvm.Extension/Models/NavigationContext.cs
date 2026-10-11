namespace CommunityToolkit.Mvvm
{
    /// <summary>
    /// 为导航生命周期回调提供上下文信息。
    /// </summary>
    public class NavigationContext
    {
        /// <summary>
        /// 初始化一个不包含导航日志引用的新导航上下文。
        /// </summary>
        /// <param name="navigationService">执行本次导航的导航服务。</param>
        /// <param name="regionName">目标区域名称。</param>
        /// <param name="viewName">目标视图名称。</param>
        /// <param name="parameter">传递给本次导航操作的参数。</param>
        public NavigationContext(INavigationService navigationService, string regionName, string viewName, Parameter parameter)
            : this(navigationService, null, regionName, viewName, parameter)
        {
        }

        /// <summary>
        /// 初始化一个新的导航上下文。
        /// </summary>
        /// <param name="navigationService">执行本次导航的导航服务。</param>
        /// <param name="journal">与目标区域关联的导航日志。</param>
        /// <param name="regionName">目标区域名称。</param>
        /// <param name="viewName">目标视图名称。</param>
        /// <param name="parameter">传递给本次导航操作的参数。</param>
        public NavigationContext(INavigationService navigationService, INavigationJournal journal, string regionName, string viewName, Parameter parameter)
        {
            NavigationService = navigationService;
            Journal = journal;
            RegionName = regionName;
            ViewName = viewName;
            Parameter = parameter ?? new Parameter();
        }

        /// <summary>
        /// 获取执行本次导航的导航服务。
        /// </summary>
        public INavigationService NavigationService { get; private set; }

        /// <summary>
        /// 获取与目标区域关联的导航日志。
        /// </summary>
        public INavigationJournal Journal { get; private set; }

        /// <summary>
        /// 获取目标区域名称。
        /// </summary>
        public string RegionName { get; private set; }

        /// <summary>
        /// 获取目标视图名称。
        /// </summary>
        public string ViewName { get; private set; }

        /// <summary>
        /// 获取传递给本次导航操作的参数。
        /// </summary>
        public Parameter Parameter { get; private set; }
    }
}
