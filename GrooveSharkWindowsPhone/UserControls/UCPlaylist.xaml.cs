using System;
using System.Linq;
using System.Reactive.Linq;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using GrooveSharkWindowsPhone.Helpers;
using GrooveSharkWindowsPhone.ViewModels;
using ReactiveUI;

namespace GrooveSharkWindowsPhone.UserControls
{
    public sealed partial class UCPlaylist : UserControl
    {
        private bool _bindingsInintialized;

        public static readonly DependencyProperty MinimizedProperty = DependencyProperty.Register("Minimized", typeof(bool), typeof(UCPlaylist), new PropertyMetadata(default(bool)));
        public static readonly DependencyProperty CanOpenProperty = DependencyProperty.Register("CanOpen", typeof(bool), typeof(UCPlaylist), new PropertyMetadata(true));
        public static readonly DependencyProperty DisableFlyoutProperty = DependencyProperty.Register("DisableFlyout", typeof(bool), typeof(UCPlaylist), new PropertyMetadata(default(bool)));

        public UCPlaylist()
        {
            this.InitializeComponent();
            Loaded += SetupBindings;
            
        }

        private void SetupBindings(object sender, RoutedEventArgs routedEventArgs)
        {
            if (Minimized)
                ThumbnailImage.Visibility = Visibility.Collapsed;
            if (!CanOpen)
                ArrowStackPanel.Visibility = Visibility.Collapsed;

            if (ViewModel == null)
                return;

            ViewModel.WhenAnyValue(vm => vm.Songs).WhereNotNull().Where(_ => CanOpen).Subscribe(s =>
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

            ViewModel.WhenAnyValue(vm => vm.IsOpen).Where(_ => CanOpen).Subscribe(x =>
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

            _bindingsInintialized = true;
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
        public bool CanOpen
        {
            get { return (bool)GetValue(CanOpenProperty); }
            set { SetValue(CanOpenProperty, value); }
        }
        public bool DisableFlyout
        {
            get { return (bool)GetValue(DisableFlyoutProperty); }
            set { SetValue(DisableFlyoutProperty, value); }
        }

        private void PlaylistGridTap(object sender, TappedRoutedEventArgs e)
        {
            if (!_bindingsInintialized)
            {
                SetupBindings(null, null);
            }
            if (CanOpen)
                ViewModel.ToggleOpenCloseCommand.Execute(null);
        }
        private void OnHolding(object sender, HoldingRoutedEventArgs e)
        {
            if (e.HoldingState != HoldingState.Started || DisableFlyout) return;

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
