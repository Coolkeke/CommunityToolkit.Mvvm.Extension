using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace CommunityToolkit.Mvvm
{
    /// <summary>
    /// 提供接近 Prism 命名风格的依赖注入扩展方法。
    /// </summary>
    /// <remarks>
    /// 这些方法只是对 <see cref="Microsoft.Extensions.DependencyInjection"/> 的轻量封装，
    /// 目的是让使用方可以用 <c>Register</c>、<c>RegisterSingleton</c>、<c>Resolve</c>
    /// 这类 Prism 常见名称来完成注册和解析。
    /// </remarks>
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// 以瞬态生命周期注册服务自身。
        /// </summary>
        /// <typeparam name="TService">要注册的服务类型。</typeparam>
        /// <param name="services">服务集合。</param>
        /// <returns>返回同一个服务集合，便于链式调用。</returns>
        public static IServiceCollection Register<TService>(this IServiceCollection services) where TService : class
        {
            return Register(services, typeof(TService), typeof(TService));
        }

        /// <summary>
        /// 以瞬态生命周期注册接口与实现类型之间的映射。
        /// </summary>
        /// <typeparam name="TFrom">服务类型，通常是接口或抽象类型。</typeparam>
        /// <typeparam name="TTo">实现类型。</typeparam>
        /// <param name="services">服务集合。</param>
        /// <returns>返回同一个服务集合，便于链式调用。</returns>
        public static IServiceCollection Register<TFrom, TTo>(this IServiceCollection services)
            where TFrom : class
            where TTo : class, TFrom
        {
            return Register(services, typeof(TFrom), typeof(TTo));
        }

        /// <summary>
        /// 以瞬态生命周期注册指定服务类型和实现类型。
        /// </summary>
        /// <param name="services">服务集合。</param>
        /// <param name="from">服务类型，通常是接口或抽象类型。</param>
        /// <param name="to">实现类型。</param>
        /// <returns>返回同一个服务集合，便于链式调用。</returns>
        public static IServiceCollection Register(this IServiceCollection services, Type from, Type to)
        {
            EnsureServices(services);
            EnsureServiceTypes(from, to);

            services.AddTransient(from, to);
            return services;
        }

        /// <summary>
        /// 以瞬态生命周期按名称注册接口与实现类型之间的映射。
        /// </summary>
        /// <typeparam name="TFrom">服务类型，通常是接口或抽象类型。</typeparam>
        /// <typeparam name="TTo">实现类型。</typeparam>
        /// <param name="services">服务集合。</param>
        /// <param name="name">注册名称。</param>
        /// <returns>返回同一个服务集合，便于链式调用。</returns>
        public static IServiceCollection Register<TFrom, TTo>(this IServiceCollection services, string name)
            where TFrom : class
            where TTo : class, TFrom
        {
            return Register(services, typeof(TFrom), typeof(TTo), name);
        }

        /// <summary>
        /// 以瞬态生命周期按名称注册服务自身。
        /// </summary>
        /// <typeparam name="TService">要注册的服务类型。</typeparam>
        /// <param name="services">服务集合。</param>
        /// <param name="name">注册名称。</param>
        /// <returns>返回同一个服务集合，便于链式调用。</returns>
        public static IServiceCollection Register<TService>(this IServiceCollection services, string name) where TService : class
        {
            return Register(services, typeof(TService), typeof(TService), name);
        }

        /// <summary>
        /// 以瞬态生命周期按名称注册指定服务类型和实现类型。
        /// </summary>
        /// <param name="services">服务集合。</param>
        /// <param name="from">服务类型，通常是接口或抽象类型。</param>
        /// <param name="to">实现类型。</param>
        /// <param name="name">注册名称。</param>
        /// <returns>返回同一个服务集合，便于链式调用。</returns>
        public static IServiceCollection Register(this IServiceCollection services, Type from, Type to, string name)
        {
            EnsureServices(services);
            EnsureServiceTypes(from, to);
            EnsureName(name);

            services.AddTransient(provider => new NamedServiceRegistration(from, name, serviceProvider => ActivatorUtilities.CreateInstance(serviceProvider, to)));
            return services;
        }

        /// <summary>
        /// 以单例生命周期注册服务自身。
        /// </summary>
        /// <typeparam name="TService">要注册的服务类型。</typeparam>
        /// <param name="services">服务集合。</param>
        /// <returns>返回同一个服务集合，便于链式调用。</returns>
        public static IServiceCollection RegisterSingleton<TService>(this IServiceCollection services) where TService : class
        {
            return RegisterSingleton(services, typeof(TService), typeof(TService));
        }

        /// <summary>
        /// 以单例生命周期注册接口与实现类型之间的映射。
        /// </summary>
        /// <typeparam name="TFrom">服务类型，通常是接口或抽象类型。</typeparam>
        /// <typeparam name="TTo">实现类型。</typeparam>
        /// <param name="services">服务集合。</param>
        /// <returns>返回同一个服务集合，便于链式调用。</returns>
        public static IServiceCollection RegisterSingleton<TFrom, TTo>(this IServiceCollection services)
            where TFrom : class
            where TTo : class, TFrom
        {
            return RegisterSingleton(services, typeof(TFrom), typeof(TTo));
        }

        /// <summary>
        /// 以单例生命周期注册指定服务类型和实现类型。
        /// </summary>
        /// <param name="services">服务集合。</param>
        /// <param name="from">服务类型，通常是接口或抽象类型。</param>
        /// <param name="to">实现类型。</param>
        /// <returns>返回同一个服务集合，便于链式调用。</returns>
        public static IServiceCollection RegisterSingleton(this IServiceCollection services, Type from, Type to)
        {
            EnsureServices(services);
            EnsureServiceTypes(from, to);

            services.AddSingleton(from, to);
            return services;
        }

        /// <summary>
        /// 以单例生命周期按名称注册接口与实现类型之间的映射。
        /// </summary>
        /// <typeparam name="TFrom">服务类型，通常是接口或抽象类型。</typeparam>
        /// <typeparam name="TTo">实现类型。</typeparam>
        /// <param name="services">服务集合。</param>
        /// <param name="name">注册名称。</param>
        /// <returns>返回同一个服务集合，便于链式调用。</returns>
        public static IServiceCollection RegisterSingleton<TFrom, TTo>(this IServiceCollection services, string name)
            where TFrom : class
            where TTo : class, TFrom
        {
            return RegisterSingleton(services, typeof(TFrom), typeof(TTo), name);
        }

        /// <summary>
        /// 以单例生命周期按名称注册服务自身。
        /// </summary>
        /// <typeparam name="TService">要注册的服务类型。</typeparam>
        /// <param name="services">服务集合。</param>
        /// <param name="name">注册名称。</param>
        /// <returns>返回同一个服务集合，便于链式调用。</returns>
        public static IServiceCollection RegisterSingleton<TService>(this IServiceCollection services, string name) where TService : class
        {
            return RegisterSingleton(services, typeof(TService), typeof(TService), name);
        }

        /// <summary>
        /// 以单例生命周期按名称注册指定服务类型和实现类型。
        /// </summary>
        /// <param name="services">服务集合。</param>
        /// <param name="from">服务类型，通常是接口或抽象类型。</param>
        /// <param name="to">实现类型。</param>
        /// <param name="name">注册名称。</param>
        /// <returns>返回同一个服务集合，便于链式调用。</returns>
        public static IServiceCollection RegisterSingleton(this IServiceCollection services, Type from, Type to, string name)
        {
            EnsureServices(services);
            EnsureServiceTypes(from, to);
            EnsureName(name);

            SingletonServiceFactory factory = new SingletonServiceFactory(to);
            services.AddSingleton(new NamedServiceRegistration(from, name, factory.Invoke));
            return services;
        }

        /// <summary>
        /// 使用已有实例注册单例服务。
        /// </summary>
        /// <typeparam name="TService">服务类型。</typeparam>
        /// <param name="services">服务集合。</param>
        /// <param name="instance">要注册的实例。</param>
        /// <returns>返回同一个服务集合，便于链式调用。</returns>
        public static IServiceCollection RegisterInstance<TService>(this IServiceCollection services, TService instance) where TService : class
        {
            EnsureServices(services);
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            services.AddSingleton<TService>(instance);
            return services;
        }

        /// <summary>
        /// 使用已有实例按名称注册单例服务。
        /// </summary>
        /// <typeparam name="TService">服务类型。</typeparam>
        /// <param name="services">服务集合。</param>
        /// <param name="instance">要注册的实例。</param>
        /// <param name="name">注册名称。</param>
        /// <returns>返回同一个服务集合，便于链式调用。</returns>
        public static IServiceCollection RegisterInstance<TService>(this IServiceCollection services, TService instance, string name) where TService : class
        {
            EnsureServices(services);
            EnsureName(name);
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            services.AddSingleton(new NamedServiceRegistration(typeof(TService), name, provider => instance));
            return services;
        }

        /// <summary>
        /// 判断服务集合中是否已经注册指定服务类型。
        /// </summary>
        /// <typeparam name="TService">要检查的服务类型。</typeparam>
        /// <param name="services">服务集合。</param>
        /// <returns>已经注册时返回 <c>true</c>；否则返回 <c>false</c>。</returns>
        public static bool IsRegistered<TService>(this IServiceCollection services)
        {
            return IsRegistered(services, typeof(TService));
        }

        /// <summary>
        /// 判断服务集合中是否已经注册指定服务类型。
        /// </summary>
        /// <param name="services">服务集合。</param>
        /// <param name="serviceType">要检查的服务类型。</param>
        /// <returns>已经注册时返回 <c>true</c>；否则返回 <c>false</c>。</returns>
        public static bool IsRegistered(this IServiceCollection services, Type serviceType)
        {
            EnsureServices(services);
            if (serviceType == null)
            {
                throw new ArgumentNullException("serviceType");
            }

            return services.Any(item => item.ServiceType == serviceType);
        }

        /// <summary>
        /// 判断服务集合中是否已经按名称注册指定服务类型。
        /// </summary>
        /// <typeparam name="TService">要检查的服务类型。</typeparam>
        /// <param name="services">服务集合。</param>
        /// <param name="name">注册名称。</param>
        /// <returns>已经注册时返回 <c>true</c>；否则返回 <c>false</c>。</returns>
        public static bool IsRegistered<TService>(this IServiceCollection services, string name)
        {
            return IsRegistered(services, typeof(TService), name);
        }

        /// <summary>
        /// 判断服务集合中是否已经按名称注册指定服务类型。
        /// </summary>
        /// <param name="services">服务集合。</param>
        /// <param name="serviceType">要检查的服务类型。</param>
        /// <param name="name">注册名称。</param>
        /// <returns>已经注册时返回 <c>true</c>；否则返回 <c>false</c>。</returns>
        public static bool IsRegistered(this IServiceCollection services, Type serviceType, string name)
        {
            EnsureServices(services);
            EnsureName(name);
            if (serviceType == null)
            {
                throw new ArgumentNullException("serviceType");
            }

            return services
                .Where(item => item.ServiceType == typeof(NamedServiceRegistration))
                .Select(CreateNamedServiceRegistration)
                .Any(item => item != null && item.ServiceType == serviceType && string.Equals(item.Name, name, StringComparison.Ordinal));
        }

        /// <summary>
        /// 从服务提供程序解析指定服务。
        /// </summary>
        /// <typeparam name="TService">要解析的服务类型。</typeparam>
        /// <param name="provider">服务提供程序。</param>
        /// <returns>解析得到的服务实例。</returns>
        public static TService Resolve<TService>(this IServiceProvider provider)
        {
            return (TService)Resolve(provider, typeof(TService));
        }

        /// <summary>
        /// 从服务提供程序解析指定服务。
        /// </summary>
        /// <param name="provider">服务提供程序。</param>
        /// <param name="serviceType">要解析的服务类型。</param>
        /// <returns>解析得到的服务实例。</returns>
        public static object Resolve(this IServiceProvider provider, Type serviceType)
        {
            EnsureProvider(provider);
            if (serviceType == null)
            {
                throw new ArgumentNullException("serviceType");
            }

            object service = provider.GetService(serviceType);
            if (service == null)
            {
                throw new InvalidOperationException("Service type '" + serviceType.FullName + "' has not been registered.");
            }

            return service;
        }

        /// <summary>
        /// 从服务提供程序按名称解析指定服务。
        /// </summary>
        /// <typeparam name="TService">要解析的服务类型。</typeparam>
        /// <param name="provider">服务提供程序。</param>
        /// <param name="name">注册名称。</param>
        /// <returns>解析得到的服务实例。</returns>
        public static TService Resolve<TService>(this IServiceProvider provider, string name)
        {
            return (TService)Resolve(provider, typeof(TService), name);
        }

        /// <summary>
        /// 从服务提供程序按名称解析指定服务。
        /// </summary>
        /// <param name="provider">服务提供程序。</param>
        /// <param name="serviceType">要解析的服务类型。</param>
        /// <param name="name">注册名称。</param>
        /// <returns>解析得到的服务实例。</returns>
        public static object Resolve(this IServiceProvider provider, Type serviceType, string name)
        {
            EnsureProvider(provider);
            EnsureName(name);
            if (serviceType == null)
            {
                throw new ArgumentNullException("serviceType");
            }

            NamedServiceRegistration registration = provider
                .GetServices<NamedServiceRegistration>()
                .LastOrDefault(item => item.ServiceType == serviceType && string.Equals(item.Name, name, StringComparison.Ordinal));

            if (registration == null)
            {
                throw new InvalidOperationException("Service type '" + serviceType.FullName + "' with name '" + name + "' has not been registered.");
            }

            return registration.Create(provider);
        }

        /// <summary>
        /// 判断服务提供程序是否能解析指定服务类型。
        /// </summary>
        /// <typeparam name="TService">要检查的服务类型。</typeparam>
        /// <param name="provider">服务提供程序。</param>
        /// <returns>可以解析时返回 <c>true</c>；否则返回 <c>false</c>。</returns>
        /// <remarks>
        /// Microsoft DI 的 <see cref="IServiceProvider"/> 不暴露注册表，因此该方法会尝试解析服务。
        /// 对瞬态服务来说，这可能创建一个临时实例。
        /// </remarks>
        public static bool IsRegistered<TService>(this IServiceProvider provider)
        {
            return IsRegistered(provider, typeof(TService));
        }

        /// <summary>
        /// 判断服务提供程序是否能解析指定服务类型。
        /// </summary>
        /// <param name="provider">服务提供程序。</param>
        /// <param name="serviceType">要检查的服务类型。</param>
        /// <returns>可以解析时返回 <c>true</c>；否则返回 <c>false</c>。</returns>
        /// <remarks>
        /// Microsoft DI 的 <see cref="IServiceProvider"/> 不暴露注册表，因此该方法会尝试解析服务。
        /// 对瞬态服务来说，这可能创建一个临时实例。
        /// </remarks>
        public static bool IsRegistered(this IServiceProvider provider, Type serviceType)
        {
            EnsureProvider(provider);
            if (serviceType == null)
            {
                throw new ArgumentNullException("serviceType");
            }

            return provider.GetService(serviceType) != null;
        }

        /// <summary>
        /// 判断服务提供程序是否能按名称解析指定服务类型。
        /// </summary>
        /// <typeparam name="TService">要检查的服务类型。</typeparam>
        /// <param name="provider">服务提供程序。</param>
        /// <param name="name">注册名称。</param>
        /// <returns>可以解析时返回 <c>true</c>；否则返回 <c>false</c>。</returns>
        public static bool IsRegistered<TService>(this IServiceProvider provider, string name)
        {
            return IsRegistered(provider, typeof(TService), name);
        }

        /// <summary>
        /// 判断服务提供程序是否能按名称解析指定服务类型。
        /// </summary>
        /// <param name="provider">服务提供程序。</param>
        /// <param name="serviceType">要检查的服务类型。</param>
        /// <param name="name">注册名称。</param>
        /// <returns>可以解析时返回 <c>true</c>；否则返回 <c>false</c>。</returns>
        public static bool IsRegistered(this IServiceProvider provider, Type serviceType, string name)
        {
            EnsureProvider(provider);
            EnsureName(name);
            if (serviceType == null)
            {
                throw new ArgumentNullException("serviceType");
            }

            return provider
                .GetServices<NamedServiceRegistration>()
                .Any(item => item.ServiceType == serviceType && string.Equals(item.Name, name, StringComparison.Ordinal));
        }

        private static void EnsureServices(IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException("services");
            }
        }

        private static void EnsureProvider(IServiceProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }
        }

        private static void EnsureServiceTypes(Type from, Type to)
        {
            if (from == null)
            {
                throw new ArgumentNullException("from");
            }

            if (to == null)
            {
                throw new ArgumentNullException("to");
            }

            if (!from.IsAssignableFrom(to))
            {
                throw new ArgumentException("Implementation type must be assignable to service type.", "to");
            }
        }

        private static void EnsureName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be empty.", "name");
            }
        }

        private sealed class NamedServiceRegistration
        {
            private readonly Func<IServiceProvider, object> factory;

            public NamedServiceRegistration(Type serviceType, string name, Func<IServiceProvider, object> factory)
            {
                ServiceType = serviceType;
                Name = name;
                this.factory = factory;
            }

            public Type ServiceType { get; private set; }

            public string Name { get; private set; }

            public object Create(IServiceProvider provider)
            {
                return factory(provider);
            }
        }

        private static NamedServiceRegistration CreateNamedServiceRegistration(ServiceDescriptor descriptor)
        {
            if (descriptor == null)
            {
                return null;
            }

            NamedServiceRegistration registration = descriptor.ImplementationInstance as NamedServiceRegistration;
            if (registration != null)
            {
                return registration;
            }

            if (descriptor.ImplementationFactory == null)
            {
                return null;
            }

            return descriptor.ImplementationFactory(EmptyServiceProvider.Instance) as NamedServiceRegistration;
        }

        private sealed class SingletonServiceFactory
        {
            private readonly Type implementationType;
            private readonly object syncRoot = new object();
            private object instance;

            public SingletonServiceFactory(Type implementationType)
            {
                this.implementationType = implementationType;
            }

            public object Invoke(IServiceProvider provider)
            {
                if (instance != null)
                {
                    return instance;
                }

                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = ActivatorUtilities.CreateInstance(provider, implementationType);
                    }

                    return instance;
                }
            }
        }

        private sealed class EmptyServiceProvider : IServiceProvider
        {
            public static readonly EmptyServiceProvider Instance = new EmptyServiceProvider();

            private EmptyServiceProvider()
            {
            }

            public object GetService(Type serviceType)
            {
                return null;
            }
        }
    }
}
