using System;

namespace CommunityToolkit.Mvvm
{
    /// <summary>
    /// 打开已注册的对话框视图，并返回其关闭结果。
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        /// 根据注册名称显示非模态对话框。
        /// </summary>
        /// <param name="dialogName">注册对话框视图时使用的名称。</param>
        void Show(string dialogName);

        /// <summary>
        /// 显示非模态对话框，并向其生命周期目标传递参数。
        /// </summary>
        /// <param name="dialogName">注册对话框视图时使用的名称。</param>
        /// <param name="parameter">传递给 <see cref="IDialogAware.OnDialogOpened"/> 的参数。</param>
        void Show(string dialogName, Parameter parameter);

        /// <summary>
        /// 显示非模态对话框，并在对话框关闭后调用回调。
        /// </summary>
        /// <param name="dialogName">注册对话框视图时使用的名称。</param>
        /// <param name="parameter">传递给 <see cref="IDialogAware.OnDialogOpened"/> 的参数。</param>
        /// <param name="callback">窗口关闭后接收关闭结果的回调。</param>
        void Show(string dialogName, Parameter parameter, Action<IDialogResult> callback);

        /// <summary>
        /// 根据注册名称显示模态对话框。
        /// </summary>
        /// <param name="dialogName">注册对话框视图时使用的名称。</param>
        /// <returns>对话框关闭时产生的结果。</returns>
        IDialogResult ShowDialog(string dialogName);

        /// <summary>
        /// 显示模态对话框，并向其生命周期目标传递参数。
        /// </summary>
        /// <param name="dialogName">注册对话框视图时使用的名称。</param>
        /// <param name="parameter">传递给 <see cref="IDialogAware.OnDialogOpened"/> 的参数。</param>
        /// <returns>对话框关闭时产生的结果。</returns>
        IDialogResult ShowDialog(string dialogName, Parameter parameter);
    }
}
