﻿using System;
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

        }


        public ReactiveList<SongViewModel> PopularSongs { get; set; }


        public ReactiveCommand<Song[]> LoadPopularSongsCommand { get; set; }

    }
}
