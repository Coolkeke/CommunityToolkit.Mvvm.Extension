using System;
using System.ComponentModel;

namespace CommunityToolkit.Mvvm
{
    /// <summary>
    /// 抽象 <see cref="IDialogService"/> 使用的 WPF 窗口承载面。
    /// </summary>
    public interface IDialogWindow
    {
        /// <summary>
        /// 获取或设置窗口承载的对话框内容。
        /// </summary>
        object Content { get; set; }

        /// <summary>
        /// 获取或设置所属窗口。
        /// </summary>
        object Owner { get; set; }

        /// <summary>
        /// 获取或设置窗口显示的标题。
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// 获取一个值，指示窗口当前是否处于活动状态。
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// 获取或设置通过 <see cref="ShowDialog"/> 显示窗口时的模态对话框结果。
        /// </summary>
        bool? DialogResult { get; set; }

        /// <summary>
        /// 在窗口关闭前发生，并允许取消关闭操作。
        /// </summary>
        event CancelEventHandler Closing;

        /// <summary>
        /// 在窗口关闭后发生。
        /// </summary>
        event EventHandler Closed;

        /// <summary>
        /// 以非模态对话框方式显示窗口。
        /// </summary>
        void Show();

        /// <summary>
        /// 以模态对话框方式显示窗口。
        /// </summary>
        /// <returns>窗口报告的可空模态结果。</returns>
        bool? ShowDialog();

        /// <summary>
        /// 关闭窗口。
        /// </summary>
        void Close();
    }
}
