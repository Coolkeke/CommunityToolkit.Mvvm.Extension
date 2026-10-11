using System.Linq;
using System.Windows.Controls;

namespace CommunityToolkit.Mvvm
{
    /// <summary>
    /// 将 <see cref="TabControl"/> 适配为单活动视图导航区域。
    /// </summary>
    public class TabControlAdapter : NavigationAdapterBase<TabControl>
    {
        /// <summary>
        /// 创建单活动区域，因为选项卡控件同一时间只有一个选中项。
        /// </summary>
        /// <param name="regionName">分配给区域的名称。</param>
        /// <returns>单活动区域。</returns>
        protected override IRegion CreateRegion(string regionName)
        {
            return new SingleActiveRegion(regionName);
        }

        /// <summary>
        /// 将选项卡项绑定到区域视图，并同步选中项与活动视图。
        /// </summary>
        /// <param name="region">其视图将作为选项卡显示的区域。</param>
        /// <param name="regionTarget">承载区域视图的选项卡控件。</param>
        protected override void Adapt(IRegion region, TabControl regionTarget)
        {
            regionTarget.ItemsSource = region.Views;

            region.ActiveViews.CollectionChanged += (sender, args) =>
            {
                var activeView = region.ActiveViews.Cast<object>().FirstOrDefault();
                if (regionTarget.SelectedItem != activeView)
                {
                    regionTarget.SelectedItem = activeView;
                }
            };

            regionTarget.SelectionChanged += (sender, args) =>
            {
                var activeView = region.ActiveViews.Cast<object>().FirstOrDefault();
                if (regionTarget.SelectedItem != activeView)
                {
                    // 选项卡点击不能绕过导航服务，否则日志、离开确认和生命周期会失去同步。
                    regionTarget.SelectedItem = activeView;
                }
            };
        }
    }
}
