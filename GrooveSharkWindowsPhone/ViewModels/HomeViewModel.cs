using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using GrooveSharkClient.Contracts;
using GrooveSharkClient.Models;
using GrooveSharkClient.Models.Entity;
using GrooveSharkWindowsPhone.Helpers;
using GrooveSharkWindowsPhone.Views;
using ReactiveUI;
using Splat;

namespace GrooveSharkWindowsPhone.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public HomeViewModel() 
        {
            PopularSongViewModel = new PopularSongViewModel();
            SearchViewModel = new SearchViewModel();

            InitCommands();
        }

        private void InitCommands()
        {
            NavigateToSettingsCommand = ReactiveCommand.Create();
            NavigateToSettingsCommand.Subscribe(_ => NavigationHelper.Navigate(typeof(SettingsView)));
        }

        public ReactiveCommand<object> NavigateToSettingsCommand { get; set; }


        private PopularSongViewModel _popularSongViewModel;
        public PopularSongViewModel PopularSongViewModel
        {
            get { return _popularSongViewModel; }
            set { this.RaiseAndSetIfChanged(ref _popularSongViewModel, value); }
        }

        private SearchViewModel _searchViewModel;
        public SearchViewModel SearchViewModel
        {
            get { return _searchViewModel; }
            set { this.RaiseAndSetIfChanged(ref _searchViewModel, value); }
        }

        

    }
}
