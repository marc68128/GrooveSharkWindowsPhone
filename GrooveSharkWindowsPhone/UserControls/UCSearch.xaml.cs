using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

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
    }
}
