namespace CommunityToolkit.Mvvm
{
    /// <summary>
    /// 描述对话框关闭操作的语义结果。
    /// </summary>
    public enum ButtonResult
    {
        /// <summary>
        /// 未提供明确结果。
        /// </summary>
        None,

        /// <summary>
        /// 对话框已确认。
        /// </summary>
        OK,

        /// <summary>
        /// 对话框已取消。
        /// </summary>
        Cancel,

        /// <summary>
        /// 对话框返回“是”。
        /// </summary>
        Yes,

        /// <summary>
        /// 对话框返回“否”。
        /// </summary>
        No
    }
}
