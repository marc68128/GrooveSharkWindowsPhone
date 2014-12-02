using System;
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
    public class LibraryViewModel : BaseViewModel
    {
        public LibraryViewModel()
        {
            UserLibrary = new ReactiveList<SongViewModel>();

            LoadUserLibraryCommand = ReactiveCommand.CreateAsyncObservable(_user.IsDataAvailableObs, _ => {
                return _user.ConnectedUserObs.Where(u => u != null && u.UserID != 0).Take(1).SelectMany(u => {
                    _loading.AddLoadingStatus("Loading library");
                    return _client.GetUserLibrarySongs(_session.SessionId);
                });
            });

            LoadUserLibraryCommand.Where(p => p != null).Subscribe(x => {
                _loading.RemoveLoadingStatus("Loading library");
                Debug.WriteLine("[LibraryViewModel] Library : " + x.Count());
                UserLibrary.Clear();
                UserLibrary.AddRange(x.Select((s, index) => new SongViewModel(s, index)));
            });

            LoadUserLibraryCommand.ThrownExceptions.OfType<WebException>().Do(_ => _loading.RemoveLoadingStatus("Loading library")).BindTo(this, self => self.WebException);
            LoadUserLibraryCommand.ThrownExceptions.OfType<GrooveSharkException>().Do(_ => _loading.RemoveLoadingStatus("Loading library")).BindTo(this, self => self.GrooveSharkException);
        }

        public ReactiveList<SongViewModel> UserLibrary { get; set; }

        public ReactiveCommand<Song[]> LoadUserLibraryCommand { get; private set; }
    }
}
