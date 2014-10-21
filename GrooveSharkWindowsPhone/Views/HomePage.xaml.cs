using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556
using GrooveSharkWindowsPhone.ViewModels;
using ReactiveUI;

namespace GrooveSharkWindowsPhone.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        Stopwatch sw = new Stopwatch(); 
        public HomePage()
        {
            this.InitializeComponent();
            
            DataContext = new HomeViewModel();

            sw.Start();
            ViewModel.LoadPopularSongsTodayCommand.Execute(null);

            var statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
            statusBar.BackgroundColor = Windows.UI.Color.FromArgb(255, 27,27,27);
            statusBar.BackgroundOpacity = 0.5;
            statusBar.ProgressIndicator.ShowAsync();

            ViewModel.WhenAnyValue(vm => vm.Status).Where(s => s != null).Subscribe(s =>
            {
                statusBar.ProgressIndicator.Text = s;
            });
            ViewModel.WhenAnyValue(vm => vm.IsLoading).Subscribe(x =>
            {
                if (x)
                    statusBar.ShowAsync();
                else
                    statusBar.HideAsync();                  
            });
            ViewModel.PopularSongsToday.Changed.Subscribe(_ => Debug.WriteLine(" !!!! " + sw.ElapsedMilliseconds));
            this.LayoutUpdated += (sender, o) => Debug.WriteLine("      ------------ >>>>>> " + sw.ElapsedMilliseconds);
        }

        private HomeViewModel ViewModel
        {
            get { return DataContext as HomeViewModel; }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
    }
}
