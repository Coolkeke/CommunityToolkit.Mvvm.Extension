using System.Windows.Controls;

namespace CommunityToolkit.Mvvm
{
    /// <summary>
    /// 将 <see cref="ItemsControl"/> 适配为单活动视图导航区域。
    /// </summary>
    public class ItemsControlAdapter : NavigationAdapterBase<ItemsControl>
    {
        /// <summary>
        /// 创建单活动区域，避免导航历史中的页面持续留在可视树中。
        /// </summary>
        /// <param name="regionName">分配给区域的名称。</param>
        /// <returns>全活动区域。</returns>
        protected override IRegion CreateRegion(string regionName)
        {
            return new SingleActiveRegion(regionName);
        }

        /// <summary>
        /// 将控件的项源绑定到当前活动视图集合。
        /// </summary>
        /// <param name="region">其视图将被显示的区域。</param>
        /// <param name="regionTarget">承载区域视图的项控件。</param>
        protected override void Adapt(IRegion region, ItemsControl regionTarget)
        {
            regionTarget.ItemsSource = region.ActiveViews;
        }
    }
}
