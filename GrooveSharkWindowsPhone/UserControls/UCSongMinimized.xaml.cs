﻿using System.Reactive.Linq;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236
using GrooveSharkWindowsPhone.ViewModels;
using ReactiveUI;

namespace GrooveSharkWindowsPhone.UserControls
{
    public sealed partial class UCSongMinimized : UserControl
    {
        public UCSongMinimized()
        {
            this.InitializeComponent();
            Loaded += (sender, args) => SetupBindings();
        }

        private void SetupBindings()
        {
            if (ViewModel == null)
                return;

            ViewModel.WhenAnyValue(vm => vm.IsFavorite)
                .Select(x => x ? Visibility.Collapsed : Visibility.Visible)
                .BindTo(AddToFavouritesFlyoutItem, item => item.Visibility);

            ViewModel.WhenAnyValue(vm => vm.IsFavorite)
                .Select(x => !x ? Visibility.Collapsed : Visibility.Visible)
                .BindTo(RemoveFromFavouritesFlyoutItem, item => item.Visibility);
        }

        private SongViewModel ViewModel
        {
            get { return DataContext as SongViewModel; }
        }

        private void OnHolding(object sender, HoldingRoutedEventArgs e)
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
    }
}
