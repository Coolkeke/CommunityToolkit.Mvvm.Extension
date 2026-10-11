namespace CommunityToolkit.Mvvm
{
    /// <summary>
    /// 同一时间只保留一个活动视图的区域实现。
    /// </summary>
    public class SingleActiveRegion : Region
    {
        /// <summary>
        /// 初始化一个新的单活动区域。
        /// </summary>
        /// <param name="name">唯一的区域名称。</param>
        public SingleActiveRegion(string name)
            : base(name)
        {
        }

        /// <summary>
        /// 清空当前活动视图集合，并激活指定视图。
        /// </summary>
        /// <param name="view">要激活的视图实例。</param>
        public override void Activate(object view)
        {
            ActiveViews.Clear();
            base.Activate(view);
        }
    }
}
