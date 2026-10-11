namespace CommunityToolkit.Mvvm
{
    /// <summary>
    /// 所有已添加视图都会保持活动状态的区域实现。
    /// </summary>
    public class AllActiveRegion : Region
    {
        /// <summary>
        /// 初始化一个新的全活动区域。
        /// </summary>
        /// <param name="name">唯一的区域名称。</param>
        public AllActiveRegion(string name)
            : base(name)
        {
        }

        /// <summary>
        /// 将视图添加到区域，并将其标记为活动视图。
        /// </summary>
        /// <param name="view">要添加的视图实例。</param>
        public override void Add(object view)
        {
            base.Add(view);
            if (!ActiveViews.Contains(view))
            {
                ActiveViews.Add(view);
            }
        }

        /// <summary>
        /// 通过将视图加入全活动区域来激活该视图。
        /// </summary>
        /// <param name="view">要激活的视图实例。</param>
        public override void Activate(object view)
        {
            Add(view);
        }
    }
}
