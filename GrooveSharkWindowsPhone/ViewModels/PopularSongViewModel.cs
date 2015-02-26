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
    public class PopularSongViewModel : LoadingViewModel<SongViewModel>
    {
        public PopularSongViewModel()
            : base("Loading popular songs...")
        {
            InitCommands();
        }
        private void InitCommands()
        {
            #region PlayAllCommand

            PlayAllCommand = ReactiveCommand.Create(_user.ConnectedUserObs.Select(c => c != null && c.UserID != 0 && c.IsAnywhere));
            PlayAllCommand.Subscribe(_ =>
            {
                foreach (var song in PopularSongs)
                {
                    song.PlayLastCommand.Execute(null);
                }
            });

            #endregion
        }


        public ReactiveList<SongViewModel> PopularSongs { get { return Data; } }

        public ReactiveCommand<object> PlayAllCommand { get; private set; }

        protected override IObservable<List<SongViewModel>> LoadData()
        {
            return _client.GetPopularSongToday(_session.SessionId).Select(l => l.Select((s, i) => new SongViewModel(s, i + 1)).ToList());
        }

        protected override IObservable<bool> CanLoadData
        {
            get { return _session.IsDataAvailableObs; }
        }
    }
}
