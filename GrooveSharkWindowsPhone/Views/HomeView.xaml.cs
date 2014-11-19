using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
using GrooveSharkWindowsPhone.ViewModels;

namespace GrooveSharkWindowsPhone.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomeView : BaseView
    {

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

            LibraryList.Loaded += SubscribeHeaderVisibility;
            PlaylistList.Loaded += SubscribeHeaderVisibility;
            FavouritesList.Loaded += SubscribeHeaderVisibility;
            PopularList.Loaded += SubscribeHeaderVisibility;
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
                HideHeader.Begin();
                _isHeaderClose = true; 
            }
            if (_isHeaderClose && (_scrollDown < -40 || scrollViewer.VerticalOffset < 80))
            {
                ShowHeader.Begin();
                _isHeaderClose = false; 
            }

            _lastOffset = scrollViewer.VerticalOffset; 

        }


        private HomeViewModel ViewModel
        {
            get { return DataContext as HomeViewModel; }
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void SelectCollectionTap(object sender, TappedRoutedEventArgs e)
        {
            RootPivot.SelectedIndex = 2;
        }

        private void SelectPlaylistTap(object sender, TappedRoutedEventArgs e)
        {
            RootPivot.SelectedIndex = 3;
        }

        private void SelectFavouritesTap(object sender, TappedRoutedEventArgs e)
        {
            RootPivot.SelectedIndex = 4;
        }

        private void SelectSearchTap(object sender, TappedRoutedEventArgs e)
        {
            RootPivot.SelectedIndex = 1;
        }

        private void SelectPopularTap(object sender, TappedRoutedEventArgs e)
        {
            RootPivot.SelectedIndex = 0;
        }

        private void PivotSelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            _scrollDown = 0;
            _lastOffset = 10000;
            if (_isHeaderClose)
            {
                ShowHeader.Begin();
                _isHeaderClose = false; 
            }
            CollectionPath.Fill = FavouritesPath.Fill = PlaylistPath.Fill = SearchPath.Fill = PopularPath.Fill = new SolidColorBrush(Colors.White);
            var grooveSharkOrange = Application.Current.Resources["GrooveSharkOrangeBrush"] as SolidColorBrush;
            switch (RootPivot.SelectedIndex)
            {
                case 0:
                    HeaderTextBlock.Text = "Popular";
                    PopularPath.Fill = grooveSharkOrange;
                    break;
                case 1:
                    HeaderTextBlock.Text = "Search";
                    SearchPath.Fill = grooveSharkOrange;
                    break;
                case 2:
                    HeaderTextBlock.Text = "Collection";
                    CollectionPath.Fill = grooveSharkOrange;
                    break;
                case 3:
                    HeaderTextBlock.Text = "Playlist";
                    PlaylistPath.Fill = grooveSharkOrange;
                    break;
                case 4:
                    HeaderTextBlock.Text = "Favourites";
                    FavouritesPath.Fill = grooveSharkOrange;
                    break;
            }
        }

    }
}
