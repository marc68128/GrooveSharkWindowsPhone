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
using Microsoft.Practices.ObjectBuilder2;
using ReactiveUI;

namespace GrooveSharkWindowsPhone.ViewModels
{
    public class AccountViewModel : BaseViewModel
    {
        public AccountViewModel()
        {
            PlaylistViewModel = new PlaylistsViewModel();
            FavouritesViewModel = new FavouritesViewModel();
            LibraryViewModel = new LibraryViewModel();

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

        
        

        private void InitCommands()
        {
            NavigateToSettingsCommand = ReactiveCommand.Create();
            NavigateToSettingsCommand.Subscribe(_ => NavigationHelper.Navigate(typeof(SettingsView)));

            NavigateToHomeCommand = ReactiveCommand.Create();
            NavigateToHomeCommand.Subscribe(_ => NavigationHelper.Navigate(typeof(HomeView)));
        }

        public ReactiveCommand<object> NavigateToSettingsCommand { get; private set; }
        public ReactiveCommand<object> NavigateToHomeCommand { get; private set; }

    }
}
