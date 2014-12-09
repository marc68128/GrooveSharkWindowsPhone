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
    public class PopularSongViewModel : BaseViewModel
    {
        public PopularSongViewModel()
        {
            PopularSongs = new ReactiveList<SongViewModel>();

            InitCommands();
        }
        private void InitCommands()
        {
            #region LoadPopularSongsTodayCommand

            LoadPopularSongsCommand = ReactiveCommand.CreateAsyncObservable(_session.IsDataAvailableObs, _ => {
                _loading.AddLoadingStatus("Loading popular songs...");
                return _client.GetPopularSongToday(_session.SessionId);
            });
            LoadPopularSongsCommand.Where(s => s != null).Subscribe(s => {
                _loading.RemoveLoadingStatus("Loading popular songs...");
                PopularSongs.Clear();
                PopularSongs.AddRange(s.Take(50).Select((x, index) => new SongViewModel(x, index + 1)));
            });

            LoadPopularSongsCommand.ThrownExceptions
                .OfType<GrooveSharkException>()
                .Do(e => _loading.RemoveLoadingStatus("Loading popular songs..."))
                .Do(e => Debug.WriteLine("[PopularSongViewModel] : " + e.Description))
                .BindTo(this, self => self.GrooveSharkException);

            LoadPopularSongsCommand.ThrownExceptions
                .OfType<WebException>()
                .Do(e => _loading.RemoveLoadingStatus("Loading popular songs..."))
                .Do(e => Debug.WriteLine("[PopularSongViewModel] : " + e.Message))
                .BindTo(this, self => self.WebException);

            #endregion

            #region PlayAllCommand

            PlayAllCommand = ReactiveCommand.Create(_user.ConnectedUserObs.Select(c => c != null  && c.UserID!= 0 && c.IsAnywhere));
            PlayAllCommand.Subscribe(_ =>
            {
                foreach (var song in PopularSongs)
                {
                    song.PlayLastCommand.Execute(null);
                }
            });

            #endregion
        }


        public ReactiveList<SongViewModel> PopularSongs { get; private set; }

        public ReactiveCommand<Song[]> LoadPopularSongsCommand { get; private set; }
        public ReactiveCommand<object> PlayAllCommand { get; private set; }

    }
}
