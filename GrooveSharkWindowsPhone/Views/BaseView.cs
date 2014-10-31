using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GrooveSharkWindowsPhone.ViewModels;
using ReactiveUI;

namespace GrooveSharkWindowsPhone.Views
{
    public class BaseView : Page
    {
        public BaseView(BaseViewModel viewModel)
        {
            DataContext = viewModel;

            var statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
            statusBar.BackgroundColor = Windows.UI.Color.FromArgb(255, 248, 111, 5);
            statusBar.BackgroundOpacity = 1;
            statusBar.ShowAsync();

            (DataContext as BaseViewModel).WhenAnyValue(vm => vm.Status).Where(s => s != null).Subscribe(s =>
            {
                statusBar.ProgressIndicator.Text = s;
            });
            (DataContext as BaseViewModel).WhenAnyValue(vm => vm.ShowLoader).Subscribe(x =>
            {
                if (x)
                    statusBar.ProgressIndicator.ShowAsync();
                else
                    statusBar.ProgressIndicator.HideAsync();
            });

            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            Frame frame = Window.Current.Content as Frame;
            if (frame == null)
            {
                return;
            }

            if (frame.CanGoBack)
            {
                frame.GoBack();
                e.Handled = true;
            }
        }
    }
}
