using System.Reactive.Linq;
using Windows.UI.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236
using GrooveSharkWindowsPhone.ViewModels;
using ReactiveUI;

namespace GrooveSharkWindowsPhone.UserControls
{
    public sealed partial class UCFavorite : UserControl
    {
        public UCFavorite()
        {
            this.InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (ViewModel == null)
                return;

            ViewModel.WhenAnyValue(vm => vm.IsFavorite)
                .Select(x => x ? Visibility.Collapsed : Visibility.Visible)
                .BindTo(AddToFavouritesFlyoutItem, item => item.Visibility);

            ViewModel.WhenAnyValue(vm => vm.IsFavorite)
                .Select(x => !x ? Visibility.Collapsed : Visibility.Visible)
                .BindTo(RemoveFromFavouritesFlyoutItem, item => item.Visibility);

            ViewModel.WhenAnyValue(vm => vm.IsFavorite)
                .Select(x => x ? 1 : 0.4)
                .BindTo(SongGrid, item => item.Opacity);  
        }

        private SongViewModel ViewModel
        {
            get { return DataContext as SongViewModel; }
        }


        private void UIElement_OnHolding(object sender, HoldingRoutedEventArgs e)
        {
            if (e.HoldingState != HoldingState.Started) return;

            var element = sender as FrameworkElement;
            if (element == null) return;

            FlyoutBase.ShowAttachedFlyout(element);
        }

        private void StartPress(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Pressed", true);
        }

        private void StopPress(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Normal", true);
        }

        private void OnTapped(object sender, TappedRoutedEventArgs e)
        {
            if (ViewModel.CurrentUser == null || ViewModel.CurrentUser.UserID == 0)
            {
                new MessageDialog(
                    "If you want to use all the features of this application (such as playlists, favourites, library and audio player) you should register or login.")
                    .ShowAsync();
                return;
            }
            if (!ViewModel.CurrentUser.IsAnywhere)
            {
                new MessageDialog(
                    "If you want to stream songs on your device, you have to upgrade your account.")
                    .ShowAsync();
                return;
            }
            ViewModel.PlayNowCommand.Execute(null);

        }
    }
}
