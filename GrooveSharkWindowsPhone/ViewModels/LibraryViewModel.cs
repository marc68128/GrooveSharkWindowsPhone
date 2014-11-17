using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using GrooveSharkClient.Models;
using ReactiveUI;

namespace GrooveSharkWindowsPhone.ViewModels
{
    public class LibraryViewModel : BaseViewModel
    {
        public LibraryViewModel()
        {
            UserLibrary = new List<SongViewModel>();

            LoadUserLibraryCommand = ReactiveCommand.CreateAsyncObservable(_user.IsDataAvailableObs, _ =>
            {
                return _user.ConnectedUserObs.Where(u => u != null && u.UserID != 0).Take(1).SelectMany(u =>
                {
                    _loading.AddLoadingStatus("Loading library");
                    return _client.GetUserLibrarySongs(_session.SessionId);
                });
            });

            LoadUserLibraryCommand.Where(p => p != null).Subscribe(x =>
            {
                _loading.RemoveLoadingStatus("Loading library");
                Debug.WriteLine("[LibraryViewModel] Library : " + x.Count());
                UserLibrary = x.Select((s, index) => new SongViewModel(s, index)).ToList();
            });

            LoadUserLibraryCommand.ThrownExceptions.OfType<WebException>().Do(_ =>  _loading.RemoveLoadingStatus("Loading library")).BindTo(this, self => self.WebException);
            LoadUserLibraryCommand.ThrownExceptions.OfType<GrooveSharkException>().Do(_ => _loading.RemoveLoadingStatus("Loading library")).BindTo(this, self => self.GrooveSharkException);
        }

        private List<SongViewModel> _userLiabrary;
        public List<SongViewModel> UserLibrary
        {
            get { return _userLiabrary; }
            set { this.RaiseAndSetIfChanged(ref _userLiabrary, value); }
        }

        

        public ReactiveCommand<Song[]> LoadUserLibraryCommand { get; private set; }
    }
}
