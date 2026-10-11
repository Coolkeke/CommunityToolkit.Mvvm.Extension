using CommunityToolkit.Mvvm;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CommunityToolkit.Mvvm
{
    public abstract class Application : System.Windows.Application
    {
        public IServiceProvider Container { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            InitializeInternal();
        }

        private void InitializeInternal()
        {
            InitIoc();
            Initialize();
            // Shell 构造期间会注册导航区域；模块初始化可以在此后安全地发起首次导航。
            Container.UseMEF();
            OnInitialized();
        }
        /// <summary>
        /// 创建Shell窗口，必须由子类实现
        /// </summary>
        /// <returns></returns>
        protected abstract Window CreateShell();
        /// <summary>
        /// 初始化Shell窗口，默认将Shell窗口设置为MainWindow，可以由子类重写
        /// </summary>
        /// <param name="shell"></param>
        protected virtual void InitializeShell(Window shell)
        { 
            MainWindow = shell; 
        }
        protected virtual void Initialize()
        {
            var shell = CreateShell();
            if (shell != null)
            {
                InitializeShell(shell);
            }
        }
        protected virtual void OnInitialized()
        {
            MainWindow?.Show();
        }
        /// <summary>
        /// 创建IOC容器，并且实例化所有的服务
        /// </summary>
        private void InitIoc()
        {
            //这里可以使用任何IOC容器，这里使用的是Microsoft.Extensions.DependencyInjection
            var services = new ServiceCollection(); 
            RegisterServices(services);//注册服务
            services.UseNavigation().UseDialog().UseMEF(CreateModule);
            Container = services.BuildServiceProvider();
        }
        /// <summary>
        /// 创建模块，子类可以重写此方法来添加自定义模块
        /// </summary>
        /// <param name="options"></param>
        protected virtual void CreateModule(MEFOptions options)
        {
        }
        protected abstract void RegisterServices(IServiceCollection services);
    }
}
