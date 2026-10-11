namespace CommunityToolkit.Mvvm
{
    /// <summary>
    /// 表示对话框关闭后返回的结果。
    /// </summary>
    public interface IDialogResult
    {
        /// <summary>
        /// 获取用于描述对话框关闭方式的按钮结果。
        /// </summary>
        ButtonResult Result { get; }

        /// <summary>
        /// 获取对话框返回的附加值。
        /// </summary>
        Parameter Parameter { get; }
    }
}
