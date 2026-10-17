using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CommunityToolkit.Mvvm
{
    /// <summary>
    /// 基于具名区域的 <see cref="INavigationService"/> WPF 实现。
    /// </summary>
    public class NavigationService : INavigationService
    {
        private static readonly ConcurrentDictionary<string, WeakReference> regionTargets = new ConcurrentDictionary<string, WeakReference>();
        private static readonly ConcurrentDictionary<NavigationService, byte> instances = new ConcurrentDictionary<NavigationService, byte>();

        private readonly IServiceProvider serviceProvider;
        private readonly Dictionary<string, IRegion> regions = new Dictionary<string, IRegion>();
        private readonly Dictionary<string, NavigationJournal> journals = new Dictionary<string, NavigationJournal>();
        private readonly Dictionary<INavigationJournalEntry, FrameworkElement> journalViews = new Dictionary<INavigationJournalEntry, FrameworkElement>();
        private readonly Dictionary<string, RegionRegistration> regionRegistrations = new Dictionary<string, RegionRegistration>();
        private readonly HashSet<string> navigationInProgress = new HashSet<string>(StringComparer.Ordinal);
        private readonly Dictionary<string, PendingNavigation> pendingNavigations = new Dictionary<string, PendingNavigation>(StringComparer.Ordinal);
        private readonly Dictionary<string, Type> viewTypes;

        /// <summary>
        /// 初始化新的导航服务。
        /// </summary>
        /// <param name="serviceProvider">用于解析已注册视图和适配器的服务提供程序。</param>
        /// <param name="registrations">可用于导航的视图注册信息。</param>
        public NavigationService(IServiceProvider serviceProvider, IEnumerable<NavigationViewRegistration> registrations)
        {
            this.serviceProvider = serviceProvider;
            viewTypes = registrations.ToDictionary(x => x.ViewName, x => x.ViewType, StringComparer.Ordinal);
            instances.TryAdd(this, 0);
        }

        /// <summary>
        /// 获取分配给 WPF 元素的附加区域名称。
        /// </summary>
        /// <param name="obj">拥有该附加属性的依赖对象。</param>
        /// <returns>已分配的区域名称。</returns>
        public static string GetRegionName(DependencyObject obj)
        {
            return (string)obj.GetValue(RegionNameProperty);
        }

        /// <summary>
        /// 设置 WPF 元素上的附加区域名称。
        /// </summary>
        /// <param name="obj">拥有该附加属性的依赖对象。</param>
        /// <param name="value">要分配的区域名称。</param>
        public static void SetRegionName(DependencyObject obj, string value)
        {
            obj.SetValue(RegionNameProperty, value);
        }

        /// <summary>
        /// 将区域导航到已注册视图。
        /// </summary>
        /// <param name="regionName">应显示目标视图的区域。</param>
        /// <param name="viewName">已注册的视图名称。</param>
        public void Navigate(string regionName, string viewName)
        {
            Navigate(regionName, viewName, null);
        }

        /// <summary>
        /// 将区域导航到已注册视图，并提供参数。
        /// </summary>
        /// <param name="regionName">应显示目标视图的区域。</param>
        /// <param name="viewName">已注册的视图名称。</param>
        /// <param name="parameter">传递给导航生命周期回调的参数。</param>
        public void Navigate(string regionName, string viewName, Parameter parameter)
        {
            var entry = new NavigationJournalEntry(regionName, viewName, parameter);
            ExecuteNavigation(entry, true, null);
        }

        /// <summary>
        /// 将区域导航到上一条日志项。
        /// </summary>
        /// <param name="regionName">需要后退导航日志的区域。</param>
        public void GoBack(string regionName)
        {
            GetJournalCore(regionName).GoBack();
        }

        /// <summary>
        /// 将区域导航到下一条日志项。
        /// </summary>
        /// <param name="regionName">需要前进导航日志的区域。</param>
        public void GoForward(string regionName)
        {
            GetJournalCore(regionName).GoForward();
        }

        /// <summary>
        /// 判断区域是否可以后退导航。
        /// </summary>
        /// <param name="regionName">需要检查导航日志的区域。</param>
        /// <returns>存在上一条日志项时返回 <c>true</c>；否则返回 <c>false</c>。</returns>
        public bool CanGoBack(string regionName)
        {
            return GetJournalCore(regionName).CanGoBack;
        }

        /// <summary>
        /// 判断区域是否可以前进导航。
        /// </summary>
        /// <param name="regionName">需要检查导航日志的区域。</param>
        /// <returns>存在下一条日志项时返回 <c>true</c>；否则返回 <c>false</c>。</returns>
        public bool CanGoForward(string regionName)
        {
            return GetJournalCore(regionName).CanGoForward;
        }

        /// <summary>
        /// 获取指定区域的导航日志。
        /// </summary>
        /// <param name="regionName">需要返回导航日志的区域。</param>
        /// <returns>该区域的导航日志。</returns>
        public INavigationJournal GetJournal(string regionName)
        {
            return GetJournalCore(regionName);
        }

        /// <summary>
        /// 解除区域注册并释放其所有页面、日志和容器引用。
        /// </summary>
        /// <param name="regionName">要释放的区域名称。</param>
        public void RemoveRegion(string regionName)
        {
            if (string.IsNullOrEmpty(regionName))
            {
                throw new ArgumentNullException(nameof(regionName));
            }

            if (regionRegistrations.TryGetValue(regionName, out var registration))
            {
                registration.Target.Unloaded -= registration.UnloadedHandler;
                registration.Target.Loaded -= registration.LoadedHandler;   // 新增
                if (registration.Window != null)
                {
                    registration.Window.Closed -= registration.ClosedHandler;
                }
                regionRegistrations.Remove(regionName);
            }

            if (regionTargets.TryGetValue(regionName, out var targetReference) &&
                (registration == null || ReferenceEquals(targetReference.Target, registration.Target)))
            {
                regionTargets.TryRemove(regionName, out _);
            }

            if (regions.TryGetValue(regionName, out var region))
            {
                foreach (var view in region.Views.ToList())
                {
                    region.Remove(view);
                }

                regions.Remove(regionName);
            }

            journals.Remove(regionName);
            foreach (var entry in journalViews.Keys.Where(item => item.RegionName == regionName).ToList())
            {
                journalViews.Remove(entry);
            }
        }

        /// <summary>
        /// 标识用于将 WPF 元素标记为导航区域的附加属性。
        /// </summary>
        public static readonly DependencyProperty RegionNameProperty =
            DependencyProperty.RegisterAttached("RegionName", typeof(string), typeof(NavigationService), new PropertyMetadata(null, OnRegionNameChanged));

        private static void OnRegionNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var oldRegionName = e.OldValue as string;
            if (!string.IsNullOrEmpty(oldRegionName))
            {
                regionTargets.TryRemove(oldRegionName, out _);
            }

            var newRegionName = e.NewValue as string;
            if (string.IsNullOrEmpty(newRegionName))
            {
                return;
            }

            var element = d as FrameworkElement;
            if (element == null)
            {
                throw new InvalidOperationException("RegionName can only be set on FrameworkElement.");
            }

            if (regionTargets.TryGetValue(newRegionName, out var existingReference))
            {
                if (existingReference.Target != null)
                {
                    throw new InvalidOperationException($"RegionName '{newRegionName}' already exists.");
                }

                regionTargets.TryRemove(newRegionName, out _);
            }

            if (!regionTargets.TryAdd(newRegionName, new WeakReference(element)))
            {
                throw new InvalidOperationException($"RegionName '{newRegionName}' already exists.");
            }

            foreach (NavigationService service in instances.Keys)
            {
                service.OnRegionRegistered(newRegionName);
            }
        }
        /// <summary>
        /// 将区域导航到已注册视图，并返回本次导航是否真正完成。
        /// </summary>
        public Task<bool> NavigateAsync(string regionName, string viewName, Parameter parameter = null)
        {
            var entry = new NavigationJournalEntry(regionName, viewName, parameter);
            return ExecuteNavigationAsync(entry, true);
        }

        private Task<bool> ExecuteNavigationAsync(INavigationJournalEntry entry, bool updateJournal)
        {
            var completion = new TaskCompletionSource<bool>();
            try
            {
                ExecuteNavigation(entry, updateJournal, succeeded => completion.TrySetResult(succeeded));
            }
            catch (Exception ex)
            {
                // 同步阶段抛出的异常（视图未注册、区域不存在等）也要进 Task，避免 await 者被永久挂起。
                completion.TrySetException(ex);
            }

            return completion.Task;
        }
        private void ExecuteNavigation(INavigationJournalEntry entry, bool updateJournal, Action<bool> completed)
        {
            if (Application.Current != null && !Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() => ExecuteNavigation(entry, updateJournal, completed)));
                return;
            }

            if (entry == null)
            {
                throw new ArgumentNullException(nameof(entry));
            }

            if (!navigationInProgress.Add(entry.RegionName))
            {
                completed?.Invoke(false);
                return;
            }

            try
            {
                if (!viewTypes.TryGetValue(entry.ViewName, out var viewType))
                {
                    throw new InvalidOperationException($"View '{entry.ViewName}' has not been registered.");
                }

                IRegion region;
                try
                {
                    region = GetRegionCore(entry.RegionName);
                }
                catch (RegionNotFoundException)
                {
                    QueuePendingNavigation(entry, updateJournal, completed);
                    navigationInProgress.Remove(entry.RegionName);
                    return;
                }
                var journal = GetJournalCore(entry.RegionName);
                var oldView = GetCurrentView(region);
                var oldEntry = journal.CurrentEntry;
                var oldContext = CreateContext(oldEntry, journal);
                var newContext = CreateContext(entry, journal);

                // 确认回调可能在对话框关闭后才返回，因此只有明确允许后才继续导航。
                ConfirmNavigationFrom(oldView, oldContext, canNavigate => RunOnUiThread(() =>
                {
                    if (!canNavigate)
                    {
                        completed?.Invoke(false);
                        navigationInProgress.Remove(entry.RegionName);
                        return;
                    }

                    FrameworkElement targetView = null;
                    var isNewView = false;
                    try
                    {
                        if (updateJournal || !journalViews.TryGetValue(entry, out targetView))
                        {
                            targetView = GetNavigationTarget(region, viewType, newContext);
                            if (targetView == null)
                            {
                                targetView = CreateView(viewType);
                                isNewView = true;
                            }
                        }

                        NotifyNavigatedFrom(oldView, oldContext);
                        region.Add(targetView);
                        region.Activate(targetView);
                        journalViews[entry] = targetView;
                        journal.UpdateCurrentEntry(entry);

                        if (isNewView)
                        {
                            NotifyInitialized(targetView, newContext);
                        }

                        NotifyNavigatedTo(targetView, newContext);
                        if (oldView != null && !ReferenceEquals(oldView, targetView) && !KeepAlive(oldView))
                        {
                            region.Remove(oldView);
                        }

                        if (updateJournal)
                        {
                            journal.RecordNavigation(entry, oldEntry);
                        }

                        completed?.Invoke(true);
                    }
                    catch
                    {
                        journal.UpdateCurrentEntry(oldEntry);
                        journalViews.Remove(entry);
                        // 已存在的目标视图可能同时被多条历史记录引用；回滚时不能销毁它。
                        if (isNewView && targetView != null && !ReferenceEquals(oldView, targetView))
                        {
                            region.Remove(targetView);
                        }

                        if (oldView != null && !ReferenceEquals(oldView, targetView))
                        {
                            try
                            {
                                region.Activate(oldView);
                                NotifyNavigatedTo(oldView, oldContext);
                            }
                            catch
                            {
                                // 原页面的恢复通知不能使导航完成信号丢失。
                            }
                        }

                        completed?.Invoke(false);
                    }
                    finally
                    {
                        navigationInProgress.Remove(entry.RegionName);
                    }
                }));
            }
            catch
            {
                navigationInProgress.Remove(entry.RegionName);
                throw;
            }
        }

        private void QueuePendingNavigation(INavigationJournalEntry entry, bool updateJournal, Action<bool> completed)
        {
            pendingNavigations[entry.RegionName] = new PendingNavigation(entry, updateJournal, completed);
        }

        private void OnRegionRegistered(string regionName)
        {
            if (!pendingNavigations.TryGetValue(regionName, out var pending))
            {
                return;
            }

            pendingNavigations.Remove(regionName);
            RunOnUiThread(() => ExecuteNavigation(pending.Entry, pending.UpdateJournal, pending.Completed));
        }

        private FrameworkElement CreateView(Type viewType)
        {
            var view = serviceProvider.GetService(viewType) ?? Activator.CreateInstance(viewType);
            var element = view as FrameworkElement;
            if (element == null)
            {
                throw new InvalidOperationException($"View '{viewType.FullName}' must inherit FrameworkElement.");
            }

            return element;
        }

        private NavigationJournal GetJournalCore(string regionName)
        {
            if (string.IsNullOrEmpty(regionName))
            {
                throw new ArgumentNullException(nameof(regionName));
            }

            if (!journals.ContainsKey(regionName))
            {
                journals[regionName] = new NavigationJournal(
                    regionName,
                    (entry, completed) => ExecuteNavigation(entry, false, completed),
                    entries => ReleaseJournalEntries(regionName, entries));
            }

            return journals[regionName];
        }

        private IRegion GetRegionCore(string regionName)
        {
            if (string.IsNullOrEmpty(regionName))
            {
                throw new ArgumentNullException(nameof(regionName));
            }

            if (regions.ContainsKey(regionName))
            {
                return regions[regionName];
            }

            if (!regionTargets.TryGetValue(regionName, out var regionTargetReference) ||
                !(regionTargetReference.Target is FrameworkElement regionTarget))
            {
                regionTargets.TryRemove(regionName, out _);
                throw new RegionNotFoundException($"RegionName '{regionName}' does not exist.");
            }

            var adapter = serviceProvider.GetServices<INavigationAdapter>()
                .Where(item => item.RegionTargetType.IsAssignableFrom(regionTarget.GetType()))
                .OrderByDescending(item => GetInheritanceDistance(regionTarget.GetType(), item.RegionTargetType))
                .FirstOrDefault();
            if (adapter == null)
            {
                throw new InvalidOperationException($"Region target '{regionTarget.GetType().FullName}' does not have a navigation adapter.");
            }

            return AddRegion(regionName, adapter.Initialize(regionTarget, regionName), regionTarget);
        }

        private NavigationContext CreateContext(INavigationJournalEntry entry, INavigationJournal journal)
        {
            if (entry == null)
            {
                return null;
            }

            return new NavigationContext(this, journal, entry.RegionName, entry.ViewName, entry.Parameter);
        }

        private static FrameworkElement GetCurrentView(IRegion region)
        {
            return region.ActiveViews.OfType<FrameworkElement>().LastOrDefault();
        }

        private static FrameworkElement GetNavigationTarget(IRegion region, Type targetViewType, NavigationContext context)
        {
            if (region == null)
            {
                return null;
            }

            foreach (FrameworkElement view in region.Views.OfType<FrameworkElement>().Where(item => item.GetType() == targetViewType).Reverse())
            {
                var aware = GetAwareTarget<INavigationAware>(view);
                if (aware != null && aware.IsNavigationTarget(context))
                {
                    return view;
                }
            }

            return null;
        }

        private static void ConfirmNavigationFrom(FrameworkElement view, NavigationContext context, Action<bool> continuation)
        {
            if (view == null || context == null)
            {
                continuation(true);
                return;
            }

            var confirm = GetAwareTarget<IConfirmNavigationRequest>(view);
            if (confirm == null)
            {
                continuation(true);
                return;
            }

            var isCompleted = 0;
            confirm.ConfirmNavigationRequest(context, result =>
            {
                if (Interlocked.Exchange(ref isCompleted, 1) == 0)
                {
                    continuation(result);
                }
            });
        }

        private static void NotifyNavigatedFrom(FrameworkElement view, NavigationContext context)
        {
            var aware = GetAwareTarget<INavigationAware>(view);
            if (aware != null && context != null)
            {
                aware.OnNavigatedFrom(context);
            }
        }

        private static void NotifyNavigatedTo(FrameworkElement view, NavigationContext context)
        {
            var aware = GetAwareTarget<INavigationAware>(view);
            if (aware != null)
            {
                aware.OnNavigatedTo(context);
            }
        }

        private static void NotifyInitialized(FrameworkElement view, NavigationContext context)
        {
            var lifecycle = GetAwareTarget<INavigationLifecycle>(view);
            if (lifecycle != null)
            {
                lifecycle.OnInitialized(context);
            }
        }

        private static bool KeepAlive(FrameworkElement view)
        {
            var lifetime = GetAwareTarget<INavigationLifetime>(view);
            return lifetime == null || lifetime.KeepAlive;
        }

        private IRegion AddRegion(string regionName, IRegion region, FrameworkElement regionTarget)
        {
            region.Views.CollectionChanged += (sender, args) =>
            {
                if (args.OldItems == null)
                {
                    return;
                }

                foreach (FrameworkElement view in args.OldItems.OfType<FrameworkElement>())
                {
                    var lifecycle = GetAwareTarget<INavigationLifecycle>(view);
                    try
                    {
                        lifecycle?.OnDestroyed();
                    }
                    catch
                    {
                        // 页面销毁失败不能阻断区域清理，否则单例服务会继续持有该页面。
                    }
                    finally
                    {
                        RemoveJournalViewReferences(view);
                    }
                }
            };

            regions[regionName] = region;
            RoutedEventHandler unloadedHandler = null;
            unloadedHandler = (sender, args) => OnRegionUnloaded(regionName);

            RoutedEventHandler loadedHandler = null;
            loadedHandler = (sender, args) => OnRegionLoaded(regionName, regionTarget);

            regionTarget.Unloaded += unloadedHandler;
            regionTarget.Loaded += loadedHandler;
            regionRegistrations[regionName] = new RegionRegistration(regionTarget, unloadedHandler, loadedHandler);
            AttachWindowClosedHandler(regionName, regionTarget);
            return region;
        }
        private void OnRegionUnloaded(string regionName)
        {
            regionTargets.TryRemove(regionName, out _);
        }

        // 重新入树时恢复映射，使区域自愈。
        private void OnRegionLoaded(string regionName, FrameworkElement regionTarget)
        {
            regionTargets[regionName] = new WeakReference(regionTarget);
            AttachWindowClosedHandler(regionName, regionTarget);
            OnRegionRegistered(regionName);
        }

        private void AttachWindowClosedHandler(string regionName, FrameworkElement regionTarget)
        {
            if (!regionRegistrations.TryGetValue(regionName, out var registration))
            {
                return;
            }

            var window = Window.GetWindow(regionTarget);
            if (ReferenceEquals(window, registration.Window))
            {
                return;
            }

            if (registration.Window != null)
            {
                registration.Window.Closed -= registration.ClosedHandler;
            }

            if (window != null)
            {
                EventHandler closedHandler = (sender, args) => RemoveRegion(regionName);
                window.Closed += closedHandler;
                registration.SetWindow(window, closedHandler);
            }
            else
            {
                registration.SetWindow(null, null);
            }
        }

        private static void RunOnUiThread(Action action)
        {
            if (Application.Current != null && !Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.BeginInvoke(action);
                return;
            }

            action();
        }
        private void RemoveJournalViewReferences(FrameworkElement view)
        {
            var entries = journalViews
                .Where(item => ReferenceEquals(item.Value, view))
                .Select(item => item.Key)
                .ToList();

            foreach (INavigationJournalEntry entry in entries)
            {
                journalViews.Remove(entry);
            }
        }

        private void ReleaseJournalEntries(string regionName, IList<INavigationJournalEntry> entries)
        {
            if (!regions.TryGetValue(regionName, out var region))
            {
                return;
            }

            var viewsToRelease = entries
                .Where(entry => journalViews.ContainsKey(entry))
                .Select(entry => journalViews[entry])
                .Distinct()
                .ToList();

            foreach (var entry in entries)
            {
                journalViews.Remove(entry);
            }

            foreach (var view in viewsToRelease)
            {
                var isStillReferenced = journalViews.Values.Any(item => ReferenceEquals(item, view));
                var isActive = region.ActiveViews.Any(item => ReferenceEquals(item, view));
                if (!isStillReferenced && !isActive)
                {
                    region.Remove(view);
                }
            }
        }

        private static int GetInheritanceDistance(Type concreteType, Type targetType)
        {
            var distance = 0;
            for (var current = concreteType; current != null; current = current.BaseType)
            {
                if (current == targetType)
                {
                    return -distance;
                }

                distance++;
            }

            return int.MinValue;
        }

        private static T GetAwareTarget<T>(FrameworkElement view) where T : class
        {
            if (view == null)
            {
                return null;
            }

            return view.DataContext as T ?? view as T;
        }

        private sealed class RegionRegistration
        {
            public RegionRegistration(FrameworkElement target, RoutedEventHandler unloadedHandler, RoutedEventHandler loadedHandler)
            {
                Target = target;
                UnloadedHandler = unloadedHandler;
                LoadedHandler = loadedHandler;
            }

            public FrameworkElement Target { get; }

            public RoutedEventHandler UnloadedHandler { get; }
            public RoutedEventHandler LoadedHandler { get; }

            public Window Window { get; private set; }

            public EventHandler ClosedHandler { get; private set; }

            public void SetWindow(Window window, EventHandler closedHandler)
            {
                Window = window;
                ClosedHandler = closedHandler;
            }
        }

        private sealed class PendingNavigation
        {
            public PendingNavigation(INavigationJournalEntry entry, bool updateJournal, Action<bool> completed)
            {
                Entry = entry;
                UpdateJournal = updateJournal;
                Completed = completed;
            }

            public INavigationJournalEntry Entry { get; }
            public bool UpdateJournal { get; }
            public Action<bool> Completed { get; }
        }

        private sealed class RegionNotFoundException : InvalidOperationException
        {
            public RegionNotFoundException(string message)
                : base(message)
            {
            }
        }
    }
}
