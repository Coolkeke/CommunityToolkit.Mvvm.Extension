using System;

namespace CommunityToolkit.Mvvm
{
    /// <summary>
    /// 允许视图或视图模型确认或取消离开自身的导航。
    /// </summary>
    public interface IConfirmNavigationRequest
    {
        /// <summary>
        /// 在导航继续之前请求确认。
        /// </summary>
        /// <param name="context">当前导航上下文。</param>
        /// <param name="continuationCallback">
        /// 必须调用的回调；传入 <c>true</c> 表示继续，传入 <c>false</c> 表示取消。
        /// </param>
        void ConfirmNavigationRequest(NavigationContext context, Action<bool> continuationCallback);
    }
}
