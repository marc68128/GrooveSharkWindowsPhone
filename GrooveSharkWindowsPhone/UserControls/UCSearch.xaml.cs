using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236
using Windows.UI.Xaml.Media.Imaging;

namespace GrooveSharkWindowsPhone.UserControls
{
    public sealed partial class UCSearch : UserControl
    {
        private double _lastOffset;
        private double _scrollDown;
        private bool _isHeaderClose;
        public event EventHandler ShowHeader;
        public event EventHandler HideHeader;

        public UCSearch()
        {
            this.InitializeComponent();
            SongList.Loaded += SubscribeHeaderVisibility;
            ArtistList.Loaded += SubscribeHeaderVisibility;
            AlbumList.Loaded += SubscribeHeaderVisibility;
            PlaylistList.Loaded += SubscribeHeaderVisibility;

            Pivot.SelectedIndex = AppSettings.GetValue("LastSearchIndex") != null ? (int)AppSettings.GetValue("LastSearchIndex") : 0;
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
                if (this.HideHeader != null)
                    this.HideHeader(this, null);
                _isHeaderClose = true;
            }
            if (_isHeaderClose && (_scrollDown < -40 || scrollViewer.VerticalOffset < 80))
            {
                if (this.ShowHeader != null)
                    this.ShowHeader(this, null);
                _isHeaderClose = false;
            }

            _lastOffset = scrollViewer.VerticalOffset;

        }
        private void PivotSelectorTap(object sender, TappedRoutedEventArgs e)
        {
            var col = (int)((sender as Button).GetValue(Grid.ColumnProperty));
            Pivot.SelectedIndex = col;
        }

        private void PivotSelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            AppSettings.AddValue("LastSearchIndex", (sender as Pivot).SelectedIndex);
           
            ImageSong.Source = new BitmapImage(new Uri("ms-appx:/Assets/Images/Icons/Song.png", UriKind.RelativeOrAbsolute));
            ImageArtist.Source = new BitmapImage(new Uri("ms-appx:/Assets/Images/Icons/Artist.png", UriKind.RelativeOrAbsolute));
            ImageAlbum.Source = new BitmapImage(new Uri("ms-appx:/Assets/Images/Icons/Album.png", UriKind.RelativeOrAbsolute));
            ImagePlaylist.Source = new BitmapImage(new Uri("ms-appx:/Assets/Images/Icons/Playlist.png", UriKind.RelativeOrAbsolute));

            switch ((sender as Pivot).SelectedIndex)
            {
                case 0:
                    ImageSong.Source = new BitmapImage(new Uri("ms-appx:/Assets/Images/Icons/SongSelected.png", UriKind.RelativeOrAbsolute));
                    break;
                case 1:
                    ImageArtist.Source = new BitmapImage(new Uri("ms-appx:/Assets/Images/Icons/ArtistSelected.png", UriKind.RelativeOrAbsolute));
                    break;
                case 2:
                    ImageAlbum.Source = new BitmapImage(new Uri("ms-appx:/Assets/Images/Icons/AlbumSelected.png", UriKind.RelativeOrAbsolute));
                    break;
                case 3:
                    ImagePlaylist.Source = new BitmapImage(new Uri("ms-appx:/Assets/Images/Icons/PlaylistSelected.png", UriKind.RelativeOrAbsolute));
                    break;
            }
        }
    }
}
