using System;

namespace CommunityToolkit.Mvvm
{
    /// <summary>
    /// 保存导航视图名称与其创建视图类型之间的映射。
    /// </summary>
    public class NavigationViewRegistration
    {
        /// <summary>
        /// 初始化一个新的导航视图注册信息。
        /// </summary>
        /// <param name="viewName">导航到该视图时使用的名称。</param>
        /// <param name="viewType">导航期间需要创建的视图类型。</param>
        public NavigationViewRegistration(string viewName, Type viewType)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                throw new ArgumentNullException(nameof(viewName));
            }

            if (viewType == null)
            {
                throw new ArgumentNullException(nameof(viewType));
            }

            ViewName = viewName;
            ViewType = viewType;
        }

        /// <summary>
        /// 获取导航到该视图时使用的名称。
        /// </summary>
        public string ViewName { get; private set; }

        /// <summary>
        /// 获取导航期间创建的视图类型。
        /// </summary>
        public Type ViewType { get; private set; }
    }
}
