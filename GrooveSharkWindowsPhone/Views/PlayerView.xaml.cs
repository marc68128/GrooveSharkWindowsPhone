using System;
using Windows.Devices.Sensors;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556
using GrooveSharkWindowsPhone.ViewModels;

namespace GrooveSharkWindowsPhone.Views
{
    public sealed partial class PlayerView
    {
        private bool _isList; 

        public PlayerView()
            : base(new PlayerViewModel())
        {
            this.InitializeComponent();
            Window.Current.SizeChanged += WindowOnSizeChanged;
        }

        private void WindowOnSizeChanged(object sender, WindowSizeChangedEventArgs windowSizeChangedEventArgs)
        {
            var b = Window.Current.Bounds;
            TitlePanel.Visibility = (b.Width > b.Height) ? Visibility.Collapsed : Visibility.Visible;
        }

        private PlayerViewModel ViewModel
        {
            get { return DataContext as PlayerViewModel; }
        }
        private void SwitchListGrid(object sender, TappedRoutedEventArgs e)
        {
            if (_isList)
            {
                _isList = false;
                ListGridImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/Images/icon_grid.jpg"));
            }
            else
            {
                _isList = true;
                ListGridImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/Images/icon_list.jpg"));
            }
            
        }
    }
}
