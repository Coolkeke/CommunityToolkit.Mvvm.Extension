using System;

namespace CommunityToolkit.Mvvm
{
    /// <summary>
    /// 定义对话框视图或其数据上下文的生命周期回调。
    /// </summary>
    public interface IDialogAware
    {
        /// <summary>
        /// 获取应应用到对话框窗口的标题。
        /// </summary>
        string Title { get; }

        /// <summary>
        /// 请求承载窗口使用指定结果关闭对话框。
        /// </summary>
        event Action<IDialogResult> RequestClose;

        /// <summary>
        /// 判断对话框当前是否允许关闭。
        /// </summary>
        /// <returns>允许关闭时返回 <c>true</c>；否则返回 <c>false</c>。</returns>
        bool CanCloseDialog();

        /// <summary>
        /// 在对话框内容创建完成且窗口显示之前调用。
        /// </summary>
        /// <param name="parameter">调用方传入的参数。</param>
        void OnDialogOpened(Parameter parameter);

        /// <summary>
        /// 在对话框窗口关闭且事件处理程序解绑之后调用。
        /// </summary>
        void OnDialogClosed();
    }
}
