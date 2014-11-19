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
            UserFavourites = new List<SongViewModel>();

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
                UserFavourites = p.Select((pl, index) => new SongViewModel(pl, index + 1, true)).ToList();
            });

            LoadUserFavouritesCommand.ThrownExceptions.OfType<WebException>().BindTo(this, self => self.WebException);
            LoadUserFavouritesCommand.ThrownExceptions.OfType<GrooveSharkException>().BindTo(this, self => self.GrooveSharkException);

           
        }



        private List<SongViewModel> _userFavourites;   
        public List<SongViewModel> UserFavourites
        {
            get { return _userFavourites; }
            set { _userFavourites = value; }
        }
        

        public ReactiveCommand<Song[]> LoadUserFavouritesCommand { get; private set; }
    }
}
