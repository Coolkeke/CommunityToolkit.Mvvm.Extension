namespace CommunityToolkit.Mvvm
{
    /// <summary>
    /// 可适配为导航区域的 WPF 控件基类。
    /// </summary>
    /// <typeparam name="T">该适配器处理的 WPF 控件类型。</typeparam>
    public abstract class NavigationAdapterBase<T> : INavigationAdapter<T>
    {
        /// <summary>
        /// 创建区域并将其连接到目标控件。
        /// </summary>
        /// <param name="regionTarget">用于显示区域视图的控件。</param>
        /// <param name="regionName">分配给区域的名称。</param>
        /// <returns>初始化后的区域。</returns>
        public IRegion Initialize(T regionTarget, string regionName)
        {
            var region = CreateRegion(regionName);
            Adapt(region, regionTarget);
            return region;
        }

        /// <summary>
        /// 创建该适配器使用的区域模型。
        /// </summary>
        /// <param name="regionName">分配给区域的名称。</param>
        /// <returns>用于绑定到控件的区域模型。</returns>
        protected abstract IRegion CreateRegion(string regionName);

        /// <summary>
        /// 将区域状态连接到目标控件。
        /// </summary>
        /// <param name="region">区域模型。</param>
        /// <param name="regionTarget">应显示该区域的控件。</param>
        protected abstract void Adapt(IRegion region, T regionTarget);
    }
}
