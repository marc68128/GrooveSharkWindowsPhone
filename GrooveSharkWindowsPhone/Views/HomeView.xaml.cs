using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556
using GrooveSharkWindowsPhone.Helpers;
using GrooveSharkWindowsPhone.ViewModels;
using ReactiveUI;

namespace GrooveSharkWindowsPhone.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomeView : BaseView
    {
        private AppBarClosedDisplayMode _appBarDefaultClosedDisplayMode;
        private double _lastOffset = 0;
        private double _scrollDown = 0;
        private bool _isHeaderClose;

        public HomeView()
            : base(new HomeViewModel())
        {
            this.InitializeComponent();
            ViewModel.PlaylistViewModel.LoadUserPlaylistsCommand.Execute(null);
            ViewModel.FavouritesViewModel.LoadUserFavouritesCommand.Execute(null);
            ViewModel.LibraryViewModel.LoadUserLibraryCommand.Execute(null);
            ViewModel.PopularSongViewModel.LoadPopularSongsCommand.Execute(null);

            SetBindings();

            LibraryList.Loaded += SubscribeHeaderVisibility;
            PlaylistList.Loaded += SubscribeHeaderVisibility;
            FavouritesList.Loaded += SubscribeHeaderVisibility;
            PopularList.Loaded += SubscribeHeaderVisibility;
        }

        private void SetBindings()
        {
            ViewModel.ShowSessionErrorObs.Select(x => x ? Visibility.Visible : Visibility.Collapsed)
                .BindTo(SessionError, s => s.Visibility);

            ViewModel.WhenAnyValue(vm => vm.ConnectedUser)
                .Select(u => (u != null && u.UserID != 0) ? Visibility.Collapsed : Visibility.Visible)
                .Subscribe(v => {
                    LibraryLoginGrid.Visibility = v;
                });

            ViewModel.FavouritesViewModel.UserFavourites.Changed.Subscribe(_ => FavouritesList.ItemsSource = ViewModel.FavouritesViewModel.UserFavourites);
            ViewModel.PlaylistViewModel.UserPlaylists.Changed.Subscribe(_ => PlaylistList.ItemsSource = ViewModel.PlaylistViewModel.UserPlaylists);
            ViewModel.LibraryViewModel.UserLibrary.Changed.Subscribe(_ => LibraryList.ItemsSource = ViewModel.LibraryViewModel.UserLibrary);
            ViewModel.PopularSongViewModel.PopularSongs.Changed.Subscribe(_ => PopularList.ItemsSource = ViewModel.PopularSongViewModel.PopularSongs);

        }

        private void SubscribeHeaderVisibility(object sender, RoutedEventArgs routedEventArgs)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(sender as UIElement); i++)
            {
                var tmp = VisualTreeHelper.GetChild(sender as UIElement, i);
                for (int j = 0; j < VisualTreeHelper.GetChildrenCount(tmp); j++)
                {
                    var tmp2 = VisualTreeHelper.GetChild(tmp, j);
                    if (tmp2 is ScrollViewer)
                    {
                        var scrollViewer = tmp2 as ScrollViewer;
                        scrollViewer.ViewChanging += ScrollViewerOnViewChanging;
                    }
                    var a = 0;
                }
            }
        }
        private void ScrollViewerOnViewChanging(object sender, ScrollViewerViewChangingEventArgs scrollViewerViewChangingEventArgs)
        {
            var scrollViewer = sender as ScrollViewer;
            var diff = scrollViewer.VerticalOffset - _lastOffset;

            if (diff >= 0 && _scrollDown >= 0)
                _scrollDown += diff;
            else if (diff < 0 && _scrollDown >= 0)
                _scrollDown = diff;
            else if (diff >= 0 && _scrollDown < 0)
                _scrollDown = diff;
            else if (diff < 0 && _scrollDown < 0)
                _scrollDown += diff;

            if (!_isHeaderClose && _scrollDown > 80)
            {
                HideHeader();
            }
            if (_isHeaderClose && (_scrollDown < -40 || scrollViewer.VerticalOffset < 80))
            {
                ShowHeader();
            }

            _lastOffset = scrollViewer.VerticalOffset;

        }

        private void ShowHeader()
        {
            ShowHeaderStoryboard.Begin();
            DownSearchStoryboard.Begin();
            _isHeaderClose = false;
            ApplicationBar.ClosedDisplayMode = AppBarClosedDisplayMode.Compact;
        }
        private void HideHeader()
        {
            HideHeaderStoryboard.Begin();
            UpSearchStoryboard.Begin();
            _isHeaderClose = true;
            ApplicationBar.ClosedDisplayMode = AppBarClosedDisplayMode.Minimal;
        }

        private HomeViewModel ViewModel
        {
            get { return DataContext as HomeViewModel; }
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        public void HeaderSelectorTap(object sender, TappedRoutedEventArgs e)
        {
            RootPivot.SelectedIndex = (int)(sender as Grid).GetValue(Grid.ColumnProperty);
        }



        private void PivotSelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            #region ShowHeader

            _scrollDown = 0;
            _lastOffset = 10000;
            if (_isHeaderClose)
            {
                ShowHeader();
            }

            #endregion

            CollectionPath.Fill = FavouritesPath.Fill = PlaylistPath.Fill = SearchPath.Fill = PopularPath.Fill = new SolidColorBrush(Colors.White);
            var grooveSharkOrange = Application.Current.Resources["GrooveSharkOrangeBrush"] as SolidColorBrush;

            switch (RootPivot.SelectedIndex)
            {
                case 0:
                    _appBarDefaultClosedDisplayMode = ApplicationBar.ClosedDisplayMode = AppBarClosedDisplayMode.Compact;
                    PlayButton.Visibility = Visibility.Visible;
                    RefreshButton.Visibility = Visibility.Visible;
                    RefreshButton.Command = ViewModel.PopularSongViewModel.LoadPopularSongsCommand;
                    HeaderTextBlock.Text = "Popular";
                    PopularPath.Fill = grooveSharkOrange;
                    break;
                case 1:
                    _appBarDefaultClosedDisplayMode = ApplicationBar.ClosedDisplayMode = AppBarClosedDisplayMode.Minimal;
                    PlayButton.Visibility = Visibility.Collapsed;
                    RefreshButton.Visibility = Visibility.Collapsed;
                    HeaderTextBlock.Text = "Search";
                    SearchPath.Fill = grooveSharkOrange;
                    break;
                case 2:
                    _appBarDefaultClosedDisplayMode = ApplicationBar.ClosedDisplayMode = AppBarClosedDisplayMode.Compact;
                    PlayButton.Visibility = Visibility.Visible;
                    RefreshButton.Visibility = Visibility.Visible;
                    RefreshButton.Command = ViewModel.LibraryViewModel.LoadUserLibraryCommand;
                    HeaderTextBlock.Text = "Collection";
                    CollectionPath.Fill = grooveSharkOrange;
                    break;
                case 3:
                    _appBarDefaultClosedDisplayMode = ApplicationBar.ClosedDisplayMode = AppBarClosedDisplayMode.Compact;
                    PlayButton.Visibility = Visibility.Collapsed;
                    RefreshButton.Visibility = Visibility.Visible;
                    RefreshButton.Command = ViewModel.PlaylistViewModel.LoadUserPlaylistsCommand;
                    HeaderTextBlock.Text = "Playlist";
                    PlaylistPath.Fill = grooveSharkOrange;
                    break;
                case 4:
                    _appBarDefaultClosedDisplayMode = ApplicationBar.ClosedDisplayMode = AppBarClosedDisplayMode.Compact;
                    PlayButton.Visibility = Visibility.Visible;
                    RefreshButton.Visibility = Visibility.Visible;
                    RefreshButton.Command = ViewModel.FavouritesViewModel.LoadUserFavouritesCommand;
                    HeaderTextBlock.Text = "Favourites";
                    FavouritesPath.Fill = grooveSharkOrange;
                    break;
            }
        }

        private void SearchShowHeader(object sender, EventArgs e)
        {
            if (_isHeaderClose)
                ShowHeader();
        }

        private void SearchHideHeader(object sender, EventArgs e)
        {
            if (!_isHeaderClose)
                HideHeader();
        }

        private void ReloadDataTap(object sender, EventArgs e)
        {
            ViewModel.ReloadAllCommand.Execute(null);
        }
    }
}
