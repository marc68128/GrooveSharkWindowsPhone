using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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

            Loaded += (sender, args) => SetupBindings();
        }

        private SongViewModel ViewModel
        {
            get { return DataContext as SongViewModel; }
        }

        private void SetupBindings()
        {
            ViewModel.WhenAnyValue(vm => vm.IsFavorite)
                .Select(x => x ? Visibility.Collapsed : Visibility.Visible)
                .BindTo(AddToFavouritesFlyoutItem, item => item.Visibility);

            ViewModel.WhenAnyValue(vm => vm.IsFavorite)
            .Select(x => !x ? Visibility.Collapsed : Visibility.Visible)
            .BindTo(RemoveFromFavouritesFlyoutItem, item => item.Visibility);
        }

        private void UIElement_OnHolding(object sender, HoldingRoutedEventArgs e)
        {
            if (e.HoldingState != HoldingState.Started) return;

            var element = sender as FrameworkElement;
            if (element == null) return;

            FlyoutBase.ShowAttachedFlyout(element);
        }
    }
}
