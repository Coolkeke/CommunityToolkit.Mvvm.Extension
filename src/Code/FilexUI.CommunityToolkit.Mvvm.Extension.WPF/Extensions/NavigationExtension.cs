using System;
using System.Linq;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace CommunityToolkit.Mvvm
{
    /// <summary>
    /// 提供区域导航相关的服务集合扩展方法。
    /// </summary>
    public static class NavigationExtension
    {
        /// <summary>
        /// 注册导航服务和默认 WPF 区域适配器。
        /// </summary>
        /// <param name="services">用于构建应用程序服务提供程序的服务集合。</param>
        /// <returns>返回同一个服务集合，便于链式调用。</returns>
        public static IServiceCollection UseNavigation(this IServiceCollection services)
        {
            services.AddSingleton<INavigationService, NavigationService>();
            services.RegisterNavigationAdapter(new TabControlAdapter());
            services.RegisterNavigationAdapter(new ContentControlAdapter());
            services.RegisterNavigationAdapter(new ItemsControlAdapter());
            return services;
        }

        /// <summary>
        /// 使用视图类型名称注册导航视图，并按命名约定自动关联视图模型。
        /// </summary>
        /// <typeparam name="TView">要注册的视图类型。</typeparam>
        /// <param name="services">用于构建应用程序服务提供程序的服务集合。</param>
        /// <returns>返回同一个服务集合，便于链式调用。</returns>
        public static IServiceCollection RegisterForNavigation<TView>(this IServiceCollection services) where TView : class
        {
            return services.RegisterForNavigation<TView>(typeof(TView).Name);
        }

        /// <summary>
        /// 使用自定义视图名称注册导航视图，并按命名约定自动关联视图模型。
        /// </summary>
        /// <typeparam name="TView">要注册的视图类型。</typeparam>
        /// <param name="services">用于构建应用程序服务提供程序的服务集合。</param>
        /// <param name="viewName">导航到该视图时使用的名称。</param>
        /// <returns>返回同一个服务集合，便于链式调用。</returns>
        public static IServiceCollection RegisterForNavigation<TView>(this IServiceCollection services, string viewName) where TView : class
        {
            var viewModelType = FindViewModelType(typeof(TView));
            RegisterForNavigation(services, typeof(TView), viewModelType, viewName);
            return services;
        }

        /// <summary>
        /// 使用自定义规则查找视图模型，并使用视图类型名称注册导航视图。
        /// </summary>
        /// <typeparam name="TView">要注册的视图类型。</typeparam>
        /// <param name="services">用于构建应用程序服务提供程序的服务集合。</param>
        /// <param name="viewModelTypeResolver">根据视图类型返回对应视图模型类型的函数。</param>
        /// <returns>返回同一个服务集合，便于链式调用。</returns>
        public static IServiceCollection RegisterForNavigation<TView>(
            this IServiceCollection services,
            Func<Type, Type> viewModelTypeResolver) where TView : class
        {
            return services.RegisterForNavigation<TView>(typeof(TView).Name, viewModelTypeResolver);
        }

        /// <summary>
        /// 使用自定义规则查找视图模型，并使用自定义视图名称注册导航视图。
        /// </summary>
        /// <typeparam name="TView">要注册的视图类型。</typeparam>
        /// <param name="services">用于构建应用程序服务提供程序的服务集合。</param>
        /// <param name="viewName">导航到该视图时使用的名称。</param>
        /// <param name="viewModelTypeResolver">根据视图类型返回对应视图模型类型的函数。</param>
        /// <returns>返回同一个服务集合，便于链式调用。</returns>
        public static IServiceCollection RegisterForNavigation<TView>(
            this IServiceCollection services,
            string viewName,
            Func<Type, Type> viewModelTypeResolver) where TView : class
        {
            if (viewModelTypeResolver == null)
            {
                throw new ArgumentNullException(nameof(viewModelTypeResolver));
            }

            var viewType = typeof(TView);
            RegisterForNavigation(services, viewType, viewModelTypeResolver(viewType), viewName);
            return services;
        }

        /// <summary>
        /// 使用视图类型名称注册导航视图，并将指定视图模型绑定到视图的 DataContext。
        /// </summary>
        /// <typeparam name="TView">要注册的视图类型。</typeparam>
        /// <typeparam name="TViewModel">导航时为视图创建的视图模型类型。</typeparam>
        /// <param name="services">用于构建应用程序服务提供程序的服务集合。</param>
        /// <returns>返回同一个服务集合，便于链式调用。</returns>
        public static IServiceCollection RegisterForNavigation<TView, TViewModel>(this IServiceCollection services)
            where TView : class
            where TViewModel : class
        {
            return services.RegisterForNavigation<TView, TViewModel>(typeof(TView).Name);
        }

        /// <summary>
        /// 使用自定义视图名称注册导航视图，并将指定视图模型绑定到视图的 DataContext。
        /// </summary>
        /// <typeparam name="TView">要注册的视图类型。</typeparam>
        /// <typeparam name="TViewModel">导航时为视图创建的视图模型类型。</typeparam>
        /// <param name="services">用于构建应用程序服务提供程序的服务集合。</param>
        /// <param name="viewName">导航到该视图时使用的名称。</param>
        /// <returns>返回同一个服务集合，便于链式调用。</returns>
        public static IServiceCollection RegisterForNavigation<TView, TViewModel>(this IServiceCollection services, string viewName)
            where TView : class
            where TViewModel : class
        {
            RegisterForNavigation(services, typeof(TView), typeof(TViewModel), viewName);
            return services;
        }

        /// <summary>
        /// 注册或替换指定 WPF 区域目标类型使用的适配器。
        /// </summary>
        /// <typeparam name="T">该适配器处理的目标控件类型。</typeparam>
        /// <param name="services">用于构建应用程序服务提供程序的服务集合。</param>
        /// <param name="adapter">为目标控件类型创建并绑定区域的适配器。</param>
        public static void RegisterNavigationAdapter<T>(this IServiceCollection services, INavigationAdapter<T> adapter)
        {
            if (adapter == null)
            {
                throw new ArgumentNullException(nameof(adapter));
            }

            var existingAdapters = services.Where(s => s.ServiceType == typeof(INavigationAdapter<T>)).ToList();
            foreach (var existingAdapter in existingAdapters)
            {
                services.Remove(existingAdapter);
            }

            var existingRuntimeAdapters = services
                .Where(s => s.ServiceType == typeof(INavigationAdapter) &&
                            (s.ImplementationInstance as INavigationAdapter)?.RegionTargetType == typeof(T))
                .ToList();
            foreach (var existingAdapter in existingRuntimeAdapters)
            {
                services.Remove(existingAdapter);
            }

            services.AddSingleton(adapter);
            services.AddSingleton<INavigationAdapter>(new NavigationAdapterRegistration<T>(adapter));
        }

        private static void RegisterForNavigation(IServiceCollection services, Type viewType, Type viewModelType, string viewName)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (string.IsNullOrWhiteSpace(viewName))
            {
                throw new ArgumentException("View name cannot be empty.", nameof(viewName));
            }

            if (!typeof(FrameworkElement).IsAssignableFrom(viewType))
            {
                throw new ArgumentException($"View '{viewType.FullName}' must inherit FrameworkElement.", nameof(viewType));
            }

            if (viewModelType != null && !services.Any(x => x.ServiceType == viewModelType))
            {
                services.AddTransient(viewModelType);
            }

            services.AddTransient(viewType, provider =>
            {
                var view = (FrameworkElement)ActivatorUtilities.CreateInstance(provider, viewType);
                if (viewModelType != null)
                {
                    view.DataContext = provider.GetRequiredService(viewModelType);
                }

                return view;
            });
            services.AddTransient(_ => new NavigationViewRegistration(viewName, viewType));
        }

        private static Type FindViewModelType(Type viewType)
        {
            var viewName = viewType.Name;
            var viewModelName = viewName.EndsWith("View", StringComparison.Ordinal)
                ? viewName + "Model"
                : viewName + "ViewModel";
            var viewNamespace = viewType.Namespace ?? string.Empty;
            var viewsSegmentIndex = viewNamespace.IndexOf(".Views.", StringComparison.Ordinal);
            var viewModelNamespace = viewsSegmentIndex >= 0
                ? viewNamespace.Substring(0, viewsSegmentIndex) + ".ViewModels." + viewNamespace.Substring(viewsSegmentIndex + ".Views.".Length)
                : viewNamespace.EndsWith(".Views", StringComparison.Ordinal)
                    ? viewNamespace.Substring(0, viewNamespace.Length - ".Views".Length) + ".ViewModels"
                    : viewNamespace;
            var fullName = string.IsNullOrEmpty(viewModelNamespace)
                ? viewModelName
                : viewModelNamespace + "." + viewModelName;

            return viewType.Assembly.GetType(fullName, false, false);
        }

        private sealed class NavigationAdapterRegistration<T> : INavigationAdapter
        {
            private readonly INavigationAdapter<T> adapter;

            public NavigationAdapterRegistration(INavigationAdapter<T> adapter)
            {
                this.adapter = adapter;
            }

            public Type RegionTargetType => typeof(T);

            public IRegion Initialize(object regionTarget, string regionName)
            {
                if (!(regionTarget is T))
                {
                    throw new ArgumentException($"Region target must be assignable to '{typeof(T).FullName}'.", nameof(regionTarget));
                }

                return adapter.Initialize((T)regionTarget, regionName);
            }
        }
    }
}
