using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using GrooveSharkClient.Models;
using GrooveSharkClient.Models.Entity;
using ReactiveUI;

namespace GrooveSharkWindowsPhone.ViewModels
{
    public class PlaylistsViewModel : BaseViewModel
    {
        public PlaylistsViewModel()
        {
            UserPlaylists = new List<PlaylistViewModel>();

            LoadUserPlaylistsCommand = ReactiveCommand.CreateAsyncObservable(_user.IsDataAvailableObs, _ =>
            {
                return _user.ConnectedUserObs.Where(u => u != null && u.UserID != 0).Take(1).SelectMany(u =>
                {
                    _loading.AddLoadingStatus("Loading playlists");
                    return _client.GetUserPlaylists(_session.SessionId);
                });
            });

            LoadUserPlaylistsCommand.Where(p => p != null).Subscribe(p =>
            {
                _loading.RemoveLoadingStatus("Loading playlists");
                Debug.WriteLine("[AccountViewModel] Playlists : " + p.Count());
                UserPlaylists = p.Select(pl => new PlaylistViewModel(pl)).ToList();
            });

            LoadUserPlaylistsCommand.ThrownExceptions.OfType<WebException>().Do(_ => _loading.RemoveLoadingStatus("Loading playlists")).BindTo(this, self => self.WebException);
            LoadUserPlaylistsCommand.ThrownExceptions.OfType<GrooveSharkException>().Do(_ => _loading.RemoveLoadingStatus("Loading playlists")).BindTo(this, self => self.GrooveSharkException);
        }

        private List<PlaylistViewModel> _userPlaylists;
        public List<PlaylistViewModel> UserPlaylists
        {
            get { return _userPlaylists; }
            set { this.RaiseAndSetIfChanged(ref _userPlaylists, value); }
        }
        

        public ReactiveCommand<Playlist[]> LoadUserPlaylistsCommand { get; private set; }
    }
}
