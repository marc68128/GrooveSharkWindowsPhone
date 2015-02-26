using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using GrooveSharkClient.Models;
using GrooveSharkClient.Models.Entity;
using GrooveSharkClient.Models.Exception;
using ReactiveUI;

namespace GrooveSharkWindowsPhone.ViewModels
{
    public class PlaylistsViewModel : LoadingViewModel<PlaylistViewModel>
    {
        public PlaylistsViewModel()
            : base("Loading playlists")
        { }

        public ReactiveList<PlaylistViewModel> UserPlaylists { get { return Data; } }


        protected override IObservable<List<PlaylistViewModel>> LoadData()
        {
             return _user.ConnectedUserObs.Where(u => u != null && u.UserID != 0).Take(1).SelectMany(u =>
             {
                 return _client.GetUserPlaylists(_session.SessionId)
                     .Select(l => l.Select(p => new PlaylistViewModel(p)).ToList());
             });
        }

        protected override IObservable<bool> CanLoadData
        {
            get { return _session.IsDataAvailableObs.CombineLatest(_user.IsDataAvailableObs, (s, u) => s & u); }
        }
    }
}
