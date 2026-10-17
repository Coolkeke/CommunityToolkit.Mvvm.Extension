using CommunityToolkit.Mvvm;
using Microsoft.Extensions.DependencyInjection;
using TestModule.ViewModels;
using TestModule.Views;

namespace TestModule
{
    public class HomeTestMudule : IModule
    {
        public void OnInitialized(IServiceProvider provider)
        {
            provider.Resolve<INavigationService>().Navigate("Main", nameof(Home));
        }

        public void RegisterServices(IServiceCollection services)
        {
            services.RegisterForNavigation<Home, HomeViewModel>(nameof(Home));
        }
    }
}
