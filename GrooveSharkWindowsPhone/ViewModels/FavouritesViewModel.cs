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
    public class FavouritesViewModel : LoadingViewModel<SongViewModel>
    {
        public FavouritesViewModel()
            : base("Loading your favourites songs...")
        {
        }

        public ReactiveList<SongViewModel> UserFavourites { get { return Data; } }


        protected override IObservable<List<SongViewModel>> LoadData()
        {
            return _user.ConnectedUserObs.Where(u => u != null && u.UserID != 0).Take(1).SelectMany(u =>
            {

                return _client.GetUserFavoriteSongs(_session.SessionId)
                        .Select(l => l
                        .Select((s,p) => new SongViewModel(s, p + 1, true))
                        .ToList());
            });
        }

        protected override IObservable<bool> CanLoadData
        {
            get { return _session.IsDataAvailableObs.CombineLatest(_user.IsDataAvailableObs, (s, u) => s & u); }
        }
    }
}
