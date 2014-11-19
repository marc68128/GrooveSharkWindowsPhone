using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using GrooveSharkClient.Models;
using ReactiveUI;

namespace GrooveSharkWindowsPhone.ViewModels
{
    public class FavouritesViewModel : BaseViewModel
    {
        public FavouritesViewModel()
        {
            UserFavourites = new ReactiveList<SongViewModel>();

            LoadUserFavouritesCommand = ReactiveCommand.CreateAsyncObservable(_user.IsDataAvailableObs, _ =>
            {
                return _user.ConnectedUserObs.Where(u => u != null && u.UserID != 0).Take(1).SelectMany(u =>
                {
                    _loading.AddLoadingStatus("Loading your favourites songs...");
                    return _client.GetUserFavoriteSongs(_session.SessionId);
                });
            });

            LoadUserFavouritesCommand.Where(p => p != null).Subscribe(p =>
            {
                _loading.RemoveLoadingStatus("Loading your favourites songs...");
                Debug.WriteLine("[FavouritesViewModel] Favourites : " + p.Count());
                UserFavourites.Clear();
                UserFavourites.AddRange(p.Select((pl, index) => new SongViewModel(pl, index + 1, true)));
            });

            LoadUserFavouritesCommand.ThrownExceptions.OfType<WebException>().BindTo(this, self => self.WebException);
            LoadUserFavouritesCommand.ThrownExceptions.OfType<GrooveSharkException>().BindTo(this, self => self.GrooveSharkException);


        }


        public ReactiveList<SongViewModel> UserFavourites { get; set; }



        public ReactiveCommand<Song[]> LoadUserFavouritesCommand { get; private set; }
    }
}
