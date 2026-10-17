using CommunityToolkit.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace View1Module.ViewModes
{
    internal class View2ViewModel : INavigationAware
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
