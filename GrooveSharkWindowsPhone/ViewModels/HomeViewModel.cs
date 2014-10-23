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
            InitCommands();

            SearchCommand = ReactiveCommand.CreateAsyncObservable(_ => _country.CountryObs.SelectMany(c => _session.SessionIdObs.SelectMany(s => _client.SearchAll("World on fire", c.GetCountryInfoAsJsonString(), s))));
            SearchCommand.BindTo(this, self => self.SearchResult);

            this.WhenAnyValue(self => self.SearchResult).Subscribe(res =>
            {
                var a = res; 
            });
            SearchCommand.Execute(null);
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

        private Tuple<Song[], Playlist[], Artist[], Album[]> _searchResult;

        public Tuple<Song[], Playlist[], Artist[], Album[]> SearchResult
        {
            get { return _searchResult; }
            set { this.RaiseAndSetIfChanged(ref _searchResult, value); }
        }

        public ReactiveCommand<Tuple<Song[], Playlist[], Artist[], Album[]>> SearchCommand { get; set; }


        

    }
}
