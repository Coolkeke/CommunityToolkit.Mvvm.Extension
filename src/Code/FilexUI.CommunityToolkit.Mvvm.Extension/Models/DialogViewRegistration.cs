using System;

namespace CommunityToolkit.Mvvm
{
    /// <summary>
    /// 保存对话框名称与其创建视图类型之间的映射。
    /// </summary>
    public class DialogViewRegistration
    {
        /// <summary>
        /// 初始化一个新的对话框注册信息。
        /// </summary>
        /// <param name="dialogName">用于打开对话框的名称。</param>
        /// <param name="viewType">为对话框创建的视图类型。</param>
        public DialogViewRegistration(string dialogName, Type viewType)
        {
            if (string.IsNullOrEmpty(dialogName))
            {
                throw new ArgumentNullException(nameof(dialogName));
            }

            if (viewType == null)
            {
                throw new ArgumentNullException(nameof(viewType));
            }

            DialogName = dialogName;
            ViewType = viewType;
        }

        /// <summary>
        /// 获取用于打开对话框的名称。
        /// </summary>
        public string DialogName { get; private set; }

        /// <summary>
        /// 获取为对话框创建的视图类型。
        /// </summary>
        public Type ViewType { get; private set; }
    }
}
