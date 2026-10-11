using System;
using System.Collections.ObjectModel;

namespace CommunityToolkit.Mvvm
{
    /// <summary>
    /// 支持多个活动视图的默认区域实现。
    /// </summary>
    public class Region : IRegion
    {
        /// <summary>
        /// 初始化一个新的区域。
        /// </summary>
        /// <param name="name">唯一的区域名称。</param>
        public Region(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name;
        }

        /// <summary>
        /// 获取唯一的区域名称。
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 获取区域当前持有的视图集合。
        /// </summary>
        public ObservableCollection<object> Views { get; } = new ObservableCollection<object>();

        /// <summary>
        /// 获取区域中当前处于活动状态的视图集合。
        /// </summary>
        public ObservableCollection<object> ActiveViews { get; } = new ObservableCollection<object>();

        /// <summary>
        /// 当视图尚未加入区域时，将其添加到区域。
        /// </summary>
        /// <param name="view">要添加的视图实例。</param>
        public virtual void Add(object view)
        {
            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            if (!Views.Contains(view))
            {
                Views.Add(view);
            }
        }

        /// <summary>
        /// 从区域移除视图，并在其处于活动状态时先停用。
        /// </summary>
        /// <param name="view">要移除的视图实例。</param>
        public virtual void Remove(object view)
        {
            if (view == null)
            {
                return;
            }

            Deactivate(view);
            Views.Remove(view);
        }

        /// <summary>
        /// 激活视图，并在需要时将其加入区域。
        /// </summary>
        /// <param name="view">要激活的视图实例。</param>
        public virtual void Activate(object view)
        {
            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            Add(view);
            if (!ActiveViews.Contains(view))
            {
                ActiveViews.Add(view);
            }
        }

        /// <summary>
        /// 停用视图，但仍将其保留在区域视图集合中。
        /// </summary>
        /// <param name="view">要停用的视图实例。</param>
        public virtual void Deactivate(object view)
        {
            if (view == null)
            {
                return;
            }

            ActiveViews.Remove(view);
        }
    }
}
