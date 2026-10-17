using CommunityToolkit.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestModule.ViewModels
{
    internal class HomeViewModel : INavigationAware
    {
        public bool IsNavigationTarget(NavigationContext context) => true;

        public void OnNavigatedFrom(NavigationContext context)
        {
           
        }

        public void OnNavigatedTo(NavigationContext context)
        {
            
        }
    }
}
