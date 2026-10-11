namespace CommunityToolkit.Mvvm
{
    /// <summary>
    /// <see cref="IDialogResult"/> 的默认实现。
    /// </summary>
    public class DialogResult : IDialogResult
    {
        /// <summary>
        /// 使用 <see cref="ButtonResult.None"/> 和空参数集合初始化新的对话框结果。
        /// </summary>
        public DialogResult()
            : this(ButtonResult.None, null)
        {
        }

        /// <summary>
        /// 使用指定按钮结果初始化新的对话框结果。
        /// </summary>
        /// <param name="result">对话框关闭的语义结果。</param>
        public DialogResult(ButtonResult result)
            : this(result, null)
        {
        }

        /// <summary>
        /// 使用按钮结果和返回参数初始化新的对话框结果。
        /// </summary>
        /// <param name="result">对话框关闭的语义结果。</param>
        /// <param name="parameter">对话框返回的附加值。</param>
        public DialogResult(ButtonResult result, Parameter parameter)
        {
            Result = result;
            Parameter = parameter ?? new Parameter();
        }

        /// <summary>
        /// 获取对话框关闭的语义结果。
        /// </summary>
        public ButtonResult Result { get; private set; }

        /// <summary>
        /// 获取对话框返回的附加值。
        /// </summary>
        public Parameter Parameter { get; private set; }
    }
}
