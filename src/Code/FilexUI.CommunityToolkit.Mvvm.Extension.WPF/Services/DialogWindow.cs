using System.Windows;

namespace CommunityToolkit.Mvvm
{
    /// <summary>
    /// 用于承载对话框内容的默认 WPF 窗口。
    /// </summary>
    public class DialogWindow : Window, IDialogWindow
    {
        /// <summary>
        /// 初始化新的对话框窗口，并默认相对所属窗口居中显示。
        /// </summary>
        public DialogWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            SizeToContent = SizeToContent.WidthAndHeight;
        }

        /// <summary>
        /// 通过 <see cref="IDialogWindow"/> 抽象获取或设置所属窗口。
        /// </summary>
        object IDialogWindow.Owner
        {
            get { return Owner; }
            set { Owner = value as Window; }
        }
    }
}
