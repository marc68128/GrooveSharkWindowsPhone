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
            statusBar.BackgroundColor = Windows.UI.Color.FromArgb(255, 27, 27, 27);
            statusBar.BackgroundOpacity = 0.5;
            statusBar.ProgressIndicator.ShowAsync();

            (DataContext as BaseViewModel).WhenAnyValue(vm => vm.Status).Where(s => s != null).Subscribe(s =>
            {
                statusBar.ProgressIndicator.Text = s;
            });
            (DataContext as BaseViewModel).WhenAnyValue(vm => vm.IsLoading).Subscribe(x =>
            {
                if (x)
                    statusBar.ShowAsync();
                else
                    statusBar.HideAsync();
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
