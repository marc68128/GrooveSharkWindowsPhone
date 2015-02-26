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
    public class LibraryViewModel : LoadingViewModel<SongViewModel>
    {
        public LibraryViewModel()
            : base("Loading library")
        {
        }

        public ReactiveList<SongViewModel> UserLibrary { get { return Data; } }

        protected override IObservable<List<SongViewModel>> LoadData()
        {
            return _user.ConnectedUserObs.Where(u => u != null && u.UserID != 0).Take(1).SelectMany(u =>
            {
                return _client.GetUserLibrarySongs(_session.SessionId).Select(l => l.Select((s, i) => new SongViewModel(s, i + 1)).ToList());
            });
        }

        protected override IObservable<bool> CanLoadData
        {
            get { return _session.IsDataAvailableObs.CombineLatest(_user.IsDataAvailableObs, (s, u) => s & u); }
        }
    }
}
