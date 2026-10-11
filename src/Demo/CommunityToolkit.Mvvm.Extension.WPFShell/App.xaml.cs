using CommunityToolkit.Mvvm.Extension.WPFShell.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Threading;

namespace CommunityToolkit.Mvvm.Extension.WPFShell
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IMEFService? _mefService;
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterServices(IServiceCollection services)
        {
            services.RegisterForNavigation<MainWindow>();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            DispatcherUnhandledException += OnDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

            try
            {
                base.OnStartup(e); 
            }
            catch (Exception exception)
            {
                TryLog($"应用程序启动失败 {exception}");
                MessageBox.Show("应用程序启动失败，请查看错误日志。", "UltraScan", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown(-1);
            }
        }
         
        protected override void OnInitialized()
        {
            _mefService = Container.Resolve<IMEFService>();
            _mefService.ModuleLoaded += OnModuleLoaded;

            Console.WriteLine($"MEF 模块发现完成，共发现 {_mefService.DiscoveredModules.Count} 个模块，自动加载 {_mefService.AutoLoadModules.Count} 个模块");

            // 自动加载发生在 Shell 创建之前，因此在此补记已加载成功的模块。
            foreach (var moduleInfo in _mefService.Modules)
            {
                LogModuleLoaded(moduleInfo);
            }

            base.OnInitialized();
        } 
        protected override void OnExit(ExitEventArgs e)
        {
            TryLog($"UltraScan 应用程序正在关闭，退出代码：{e.ApplicationExitCode}");

            if (_mefService is not null)
            {
                _mefService.ModuleLoaded -= OnModuleLoaded;
            }

            DispatcherUnhandledException -= OnDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;
            TaskScheduler.UnobservedTaskException -= OnUnobservedTaskException; 
            base.OnExit(e);
        }

        private void OnModuleLoaded(IModuleInfo moduleInfo)
        {
            LogModuleLoaded(moduleInfo);
        }

        private void LogModuleLoaded(IModuleInfo moduleInfo)
        {
            var description = string.IsNullOrWhiteSpace(moduleInfo.Description) ? "无" : moduleInfo.Description;

            TryLog($"MEF 模块加载成功：名称={moduleInfo.Name}，类型={moduleInfo.ModuleType.FullName}，说明={description}");
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            TryLog($"UI 线程发生未处理异常 {e.Exception}");

            e.Handled = true;
            MessageBox.Show("程序发生未处理异常，即将退出，请查看错误日志。", "UltraScan", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown(-1);
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception exception)
            {
                TryLog($"非 UI 线程发生未处理异常 {exception}");
                return;
            }

            TryLog($"发生未处理的非 UI 异常：{e.ExceptionObject}");
        }

        private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            TryLog($"任务发生未观察异常 {e.Exception}");
            e.SetObserved();
        }

        private void TryLog(string message)
        {
            try
            {
                Console.WriteLine(message);
            }
            catch
            {
                // 异常处理器不能因日志组件故障而抛出新的异常。
            }
        }
    }

}
