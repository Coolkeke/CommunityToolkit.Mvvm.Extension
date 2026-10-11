namespace CommunityToolkit.Mvvm
{
    /// <summary>
    /// <see cref="INavigationJournalEntry"/> 的默认实现。
    /// </summary>
    public class NavigationJournalEntry : INavigationJournalEntry
    {
        /// <summary>
        /// 初始化一条新的导航日志记录。
        /// </summary>
        /// <param name="regionName">发生导航的区域。</param>
        /// <param name="viewName">导航到的视图名称。</param>
        /// <param name="parameter">与本次导航关联的参数。</param>
        public NavigationJournalEntry(string regionName, string viewName, Parameter parameter)
        {
            if (string.IsNullOrEmpty(regionName))
            {
                throw new System.ArgumentNullException(nameof(regionName));
            }

            if (string.IsNullOrEmpty(viewName))
            {
                throw new System.ArgumentNullException(nameof(viewName));
            }

            RegionName = regionName;
            ViewName = viewName;
            Parameter = parameter ?? new Parameter();
        }

        /// <summary>
        /// 获取发生导航的区域。
        /// </summary>
        public string RegionName { get; private set; }

        /// <summary>
        /// 获取导航到的视图名称。
        /// </summary>
        public string ViewName { get; private set; }

        /// <summary>
        /// 获取与本次导航关联的参数。
        /// </summary>
        public Parameter Parameter { get; private set; }
    }
}
