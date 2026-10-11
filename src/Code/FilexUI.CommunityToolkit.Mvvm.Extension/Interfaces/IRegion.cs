using System.Collections.ObjectModel;

namespace CommunityToolkit.Mvvm
{
    /// <summary>
    /// 表示一个具名导航区域，用于承载视图并跟踪活动视图。
    /// </summary>
    public interface IRegion
    {
        /// <summary>
        /// 获取唯一的区域名称。
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 获取区域当前持有的视图集合。
        /// </summary>
        ObservableCollection<object> Views { get; }

        /// <summary>
        /// 获取区域中当前处于活动状态的视图集合。
        /// </summary>
        ObservableCollection<object> ActiveViews { get; }

        /// <summary>
        /// 当视图尚未加入区域时，将其添加到区域。
        /// </summary>
        /// <param name="view">要添加的视图实例。</param>
        void Add(object view);

        /// <summary>
        /// 从区域移除视图，并在必要时将其停用。
        /// </summary>
        /// <param name="view">要移除的视图实例。</param>
        void Remove(object view);

        /// <summary>
        /// 将视图标记为区域中的活动视图。
        /// </summary>
        /// <param name="view">要激活的视图实例。</param>
        void Activate(object view);

        /// <summary>
        /// 将视图标记为区域中的非活动视图。
        /// </summary>
        /// <param name="view">要停用的视图实例。</param>
        void Deactivate(object view);
    }
}
