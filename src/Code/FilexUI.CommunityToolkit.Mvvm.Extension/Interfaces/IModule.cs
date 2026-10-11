using System;
using Microsoft.Extensions.DependencyInjection;

namespace CommunityToolkit.Mvvm
{
    /// <summary>
    /// 表示一个可以提供服务注册并执行启动逻辑的应用程序模块。
    /// </summary>
    /// <remarks>
    /// <see cref="RegisterServices(IServiceCollection)"/> 会在根
    /// <see cref="IServiceProvider"/> 构建前调用，因此其中的注册会进入宿主应用的依赖注入容器。
    /// <see cref="OnInitialized"/> 只会在模块被激活时调用。
    /// 标记为 <see cref="ModuleAttribute.AutoLoad"/> 等于 <c>false</c> 的模块需要手动激活。
    /// </remarks>
    public interface IModule
    {
        /// <summary>
        /// 在服务提供程序构建前注册服务。
        /// </summary>
        /// <param name="services">
        /// 应用程序服务集合，后续会用于构建根服务提供程序。
        /// </param>
        void RegisterServices(IServiceCollection services);

        /// <summary>
        /// 在服务提供程序就绪后初始化模块。
        /// </summary>
        /// <param name="provider">
        /// 根服务提供程序，包含宿主服务以及已发现模块注册的服务。
        /// </param>
        void OnInitialized(IServiceProvider provider);
    }
}
