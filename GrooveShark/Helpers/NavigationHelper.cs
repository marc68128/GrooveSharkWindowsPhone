using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace GrooveSharkWindowsPhone.Helpers
{
    public static class NavigationHelper 
    {
        private static Frame Frame { get { return (Frame)Window.Current.Content; } }

        public static bool Navigate(Type sourcePageType)
        {
            return Frame.Navigate(sourcePageType);
        }
        public static void Navigate(Type sourcePageType, object parameter)
        {
            Frame.Navigate(sourcePageType, parameter);
        }

        public static void ClearStack()
        {
            ((Frame)Window.Current.Content).BackStack.Clear();
        }

        public static void GoBack()
        {
            if (Frame != null && Frame.CanGoBack) Frame.GoBack();
        }

        public static bool CanGoBack()
        {
            return Frame != null && Frame.CanGoBack;
        }
    }
}
