using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

using GrooveSharkClient.Models;
using GrooveSharkClient.Models.Entity;
using GrooveSharkWindowsPhone.Helpers;
using GrooveSharkWindowsPhone.ViewModels;
using Microsoft.Practices.ObjectBuilder2;
using ReactiveUI;

namespace GrooveSharkWindowsPhone.UserControls
{
    public sealed partial class UCPlaylist : UserControl
    {
        private bool isOpen;
        public static readonly DependencyProperty MinimizedProperty = DependencyProperty.Register("Minimized", typeof (bool), typeof (UCPlaylist), new PropertyMetadata(default(bool)));

        public UCPlaylist()
        {
            this.InitializeComponent();
            Loaded += SetupBindings;
        }

        private void SetupBindings(object sender, RoutedEventArgs routedEventArgs)
        {
            if (Minimized)
                ThumbnailImage.Visibility = Visibility.Collapsed;

            ViewModel.WhenAnyValue(vm => vm.Songs).WhereNotNull().Subscribe(s =>
            {
                SongStackPanel.Children.Clear();
                int i = 1;
                foreach (var song in ViewModel.Songs)
                {
                    SongStackPanel.Children.Add(new UCSongMinimized
                    {
                        DataContext = new SongViewModel(song, i++),
                        Margin = new Thickness(50, 0, 0, 0)
                    });
                }
            });

            ViewModel.WhenAnyValue(vm => vm.IsOpen).Subscribe(x =>
            {
                if (x)
                {
                    OpenDoubleAnimation.To = 30 * ViewModel.Songs.Count();
                    OpenSongs.Begin();
                    RotateArrow.Begin();
                }
                else
                {
                    CloseSongs.Begin();
                    ReverseRotateArrow.Begin();
                }
            });
        }


        private PlaylistViewModel ViewModel
        {
            get { return DataContext as PlaylistViewModel; }
        }

        public bool Minimized
        {
            get { return (bool) GetValue(MinimizedProperty); }
            set { SetValue(MinimizedProperty, value); }
        }

        private void PlaylistGridTap(object sender, TappedRoutedEventArgs e)
        {
            ViewModel.ToggleOpenCloseCommand.Execute(null);
        }

        private void OnHolding(object sender, HoldingRoutedEventArgs e)
        {
            if (e.HoldingState != HoldingState.Started) return;

            var element = sender as FrameworkElement;
            if (element == null) return;

            FlyoutBase.ShowAttachedFlyout(element);
        }
    }
}
