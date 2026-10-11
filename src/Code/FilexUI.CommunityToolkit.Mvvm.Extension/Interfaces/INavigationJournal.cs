namespace CommunityToolkit.Mvvm
{
    /// <summary>
    /// 跟踪单个区域的导航历史。
    /// </summary>
    public interface INavigationJournal
    {
        /// <summary>
        /// 获取一个值，指示是否存在上一条导航日志项。
        /// </summary>
        bool CanGoBack { get; }

        /// <summary>
        /// 获取一个值，指示是否存在下一条导航日志项。
        /// </summary>
        bool CanGoForward { get; }

        /// <summary>
        /// 获取当前导航日志项。
        /// </summary>
        INavigationJournalEntry CurrentEntry { get; }

        /// <summary>
        /// 在可用时导航到上一条日志项。
        /// </summary>
        void GoBack();

        /// <summary>
        /// 在可用时导航到下一条日志项。
        /// </summary>
        void GoForward();

        /// <summary>
        /// 清空后退和前进历史堆栈。
        /// </summary>
        void Clear();
    }
}
