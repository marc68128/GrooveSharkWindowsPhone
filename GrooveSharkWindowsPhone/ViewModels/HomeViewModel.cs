using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using GrooveSharkClient.Models;
using GrooveSharkClient.Models.Entity;
using GrooveSharkWindowsPhone.Helpers;
using GrooveSharkWindowsPhone.Views;
using ReactiveUI;

namespace GrooveSharkWindowsPhone.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public HomeViewModel()
        {
            PlaylistViewModel = new PlaylistsViewModel();
            FavouritesViewModel = new FavouritesViewModel();
            LibraryViewModel = new LibraryViewModel();
            PopularSongViewModel = new PopularSongViewModel();
            SearchViewModel = new SearchViewModel();

            _user.IsDataAvailableObs.Where(x => !x).Subscribe(_ => new MessageDialog("You have to be connected to access this screen !"));
            _user.ConnectedUserObs.BindTo(this, self => self.ConnectedUser);
            InitCommands();
        
        }

        private User _connectedUser;
        public User ConnectedUser
        {
            get { return _connectedUser; }
            private set { this.RaiseAndSetIfChanged(ref _connectedUser, value); }
        }

        #region ViewModels

        private PlaylistsViewModel _playlistViewModel;
        public PlaylistsViewModel PlaylistViewModel
        {
            get { return _playlistViewModel; }
            set { this.RaiseAndSetIfChanged(ref _playlistViewModel, value); }
        }

        private FavouritesViewModel _favouritesViewModel;
        public FavouritesViewModel FavouritesViewModel
        {
            get { return _favouritesViewModel; }
            set { this.RaiseAndSetIfChanged(ref _favouritesViewModel, value); }
        }

        private LibraryViewModel _libraryViewModel;
        public LibraryViewModel LibraryViewModel
        {
            get { return _libraryViewModel; }
            set { this.RaiseAndSetIfChanged(ref _libraryViewModel, value); }
        }

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

        #endregion

        private void InitCommands()
        {
            NavigateToSettingsCommand = ReactiveCommand.Create();
            NavigateToSettingsCommand.Subscribe(_ => NavigationHelper.Navigate(typeof (SettingsView)));

            NavigateToLoginCommand = ReactiveCommand.Create();
            NavigateToLoginCommand.Subscribe(_ => NavigationHelper.Navigate(typeof(LoginView)));

            ReloadAllCommand = ReactiveCommand.Create();
            ReloadAllCommand.Subscribe(_ =>
            {
                _session.LoadSessionId.Execute(null);
                _country.LoadCountryCommand.Execute(null);
                PopularSongViewModel.LoadPopularSongsCommand.Execute(null);
                LibraryViewModel.LoadUserLibraryCommand.Execute(null);
                FavouritesViewModel.LoadUserFavouritesCommand.Execute(null);
                PlaylistViewModel.LoadUserPlaylistsCommand.Execute(null);
            });
        }

        public ReactiveCommand<object> NavigateToLoginCommand { get; private set; }
        public ReactiveCommand<object> NavigateToSettingsCommand { get; private set; }
        public ReactiveCommand<object> ReloadAllCommand { get; private set; }


    }
}
