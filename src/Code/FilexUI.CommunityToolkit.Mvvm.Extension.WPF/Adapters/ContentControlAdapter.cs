using System.Linq;
using System.Windows.Controls;

namespace CommunityToolkit.Mvvm
{
    /// <summary>
    /// 将 <see cref="ContentControl"/> 适配为单活动视图导航区域。
    /// </summary>
    public class ContentControlAdapter : NavigationAdapterBase<ContentControl>
    {
        /// <summary>
        /// 创建单活动区域，因为内容控件同一时间只能显示一个内容对象。
        /// </summary>
        /// <param name="regionName">分配给区域的名称。</param>
        /// <returns>单活动区域。</returns>
        protected override IRegion CreateRegion(string regionName)
        {
            return new SingleActiveRegion(regionName);
        }

        /// <summary>
        /// 每当活动视图集合变化时更新控件内容。
        /// </summary>
        /// <param name="region">其活动视图将被显示的区域。</param>
        /// <param name="regionTarget">承载活动视图的内容控件。</param>
        protected override void Adapt(IRegion region, ContentControl regionTarget)
        {
            region.ActiveViews.CollectionChanged += (sender, args) => UpdateContent(region, regionTarget);
            UpdateContent(region, regionTarget);
        }

        private static void UpdateContent(IRegion region, ContentControl regionTarget)
        {
            regionTarget.Content = region.ActiveViews.Cast<object>().FirstOrDefault();
        }
    }
}
