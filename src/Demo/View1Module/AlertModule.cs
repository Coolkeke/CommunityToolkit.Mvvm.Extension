using CommunityToolkit.Mvvm;
using Microsoft.Extensions.DependencyInjection;
using View1Module.ViewModes;
using View1Module.Views;

namespace View1Module
{
    public class AlertModule : IModule
    {
        public void OnInitialized(IServiceProvider provider)
        { 
            provider.Resolve<INavigationService>().Navigate("Nav_Home1", nameof(View1));
            provider.Resolve<INavigationService>().Navigate("Nav_Home2", nameof(View2));
            provider.Resolve<INavigationService>().Navigate("Nav_Home3", nameof(View2));
            provider.Resolve<INavigationService>().Navigate("Nav_Home4", nameof(View1));
        }

        public void RegisterServices(IServiceCollection services)
        {
            services.RegisterForNavigation<View1, View1ViewModel>(nameof(View1));
            services.RegisterForNavigation<View2, View2ViewModel>(nameof(View2));
        }
    }
}
