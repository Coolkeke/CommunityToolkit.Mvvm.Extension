namespace CommunityToolkit.Mvvm
{
    /// <summary>
    /// 描述区域导航日志中的一条导航记录。
    /// </summary>
    public interface INavigationJournalEntry
    {
        /// <summary>
        /// 获取目标区域名称。
        /// </summary>
        string RegionName { get; }

        /// <summary>
        /// 获取已注册视图名称。
        /// </summary>
        string ViewName { get; }

        /// <summary>
        /// 获取与导航记录关联的参数。
        /// </summary>
        Parameter Parameter { get; }
    }
}
