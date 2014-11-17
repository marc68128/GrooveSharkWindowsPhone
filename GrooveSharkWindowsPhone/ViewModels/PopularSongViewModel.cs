using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            PopularSongsToday = new ReactiveList<SongViewModel>();

            InitCommands();
        }
        private void InitCommands()
        {
            #region LoadPopularSongsTodayCommand

            LoadPopularSongsTodayCommand =  ReactiveCommand.CreateAsyncObservable( _session.IsDataAvailableObs, _ =>
            {
                _loading.AddLoadingStatus("Loading popular songs...");
                return _client.GetPopularSongToday(_session.SessionId); 
            });
            LoadPopularSongsTodayCommand.Where(s => s != null).Subscribe(s =>
            {
                _loading.RemoveLoadingStatus("Loading popular songs..."); 
                PopularSongsToday.Clear();
                PopularSongsToday.AddRange(s.Take(50).Select((x, index) => new SongViewModel(x, index + 1)));
            });
            LoadPopularSongsTodayCommand.ThrownExceptions
                .OfType<GrooveSharkException>()
                .Do(e =>  _loading.RemoveLoadingStatus("Loading popular songs..."))
                .Do(e => Debug.WriteLine("[PopularSongViewModel] : " + e.Description))
                .BindTo(this, self => self.GrooveSharkException);

            #endregion
      
        }

        public ReactiveList<SongViewModel> PopularSongsToday { get; set; }
        public ReactiveCommand<Song[]> LoadPopularSongsTodayCommand { get; set; }

    }
}
