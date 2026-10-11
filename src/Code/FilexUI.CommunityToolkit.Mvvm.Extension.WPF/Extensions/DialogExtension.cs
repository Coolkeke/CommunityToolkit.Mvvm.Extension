using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace CommunityToolkit.Mvvm
{
    /// <summary>
    /// 提供 WPF 对话框相关的服务集合扩展方法。
    /// </summary>
    public static class DialogExtension
    {
        /// <summary>
        /// 注册默认对话框服务和默认对话框窗口实现。
        /// </summary>
        /// <param name="services">用于构建应用程序服务提供程序的服务集合。</param>
        /// <returns>返回同一个服务集合，便于链式调用。</returns>
        public static IServiceCollection UseDialog(this IServiceCollection services)
        {
            services.AddSingleton<IDialogService, DialogService>();
            services.AddTransient<IDialogWindow, DialogWindow>();
            return services;
        }

        /// <summary>
        /// 使用视图类型名称注册对话框视图。
        /// </summary>
        /// <typeparam name="TView">对话框视图类型。</typeparam>
        /// <param name="services">用于构建应用程序服务提供程序的服务集合。</param>
        /// <returns>返回同一个服务集合，便于链式调用。</returns>
        public static IServiceCollection RegisterForDialog<TView>(this IServiceCollection services) where TView : class
        {
            return services.RegisterForDialog<TView>(typeof(TView).Name);
        }

        /// <summary>
        /// 使用自定义对话框名称注册对话框视图。
        /// </summary>
        /// <typeparam name="TView">对话框视图类型。</typeparam>
        /// <param name="services">用于构建应用程序服务提供程序的服务集合。</param>
        /// <param name="dialogName">打开对话框时使用的名称。</param>
        /// <returns>返回同一个服务集合，便于链式调用。</returns>
        public static IServiceCollection RegisterForDialog<TView>(this IServiceCollection services, string dialogName) where TView : class
        {
            services.AddTransient<TView>();
            services.AddSingleton(new DialogViewRegistration(dialogName, typeof(TView)));
            return services;
        }

        /// <summary>
        /// 替换默认对话框窗口实现。
        /// </summary>
        /// <typeparam name="TWindow">用于承载对话框内容的自定义窗口类型。</typeparam>
        /// <param name="services">用于构建应用程序服务提供程序的服务集合。</param>
        /// <returns>返回同一个服务集合，便于链式调用。</returns>
        public static IServiceCollection RegisterDialogWindow<TWindow>(this IServiceCollection services) where TWindow : class, IDialogWindow
        {
            var existingDescriptors = services.Where(x => x.ServiceType == typeof(IDialogWindow)).ToList();
            foreach (var descriptor in existingDescriptors)
            {
                services.Remove(descriptor);
            }

            services.AddTransient<IDialogWindow, TWindow>();
            return services;
        }
    }
}
