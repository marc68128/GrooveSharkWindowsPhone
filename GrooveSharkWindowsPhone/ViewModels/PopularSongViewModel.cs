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
            PopularSongs = new List<SongViewModel>();

            InitCommands();
        }
        private void InitCommands()
        {
            #region LoadPopularSongsTodayCommand

            LoadPopularSongsCommand = ReactiveCommand.CreateAsyncObservable(_session.IsDataAvailableObs, _ =>
            {
                _loading.AddLoadingStatus("Loading popular songs...");
                return _client.GetPopularSongToday(_session.SessionId); 
            });
            LoadPopularSongsCommand.Where(s => s != null).Subscribe(s =>
            {
                _loading.RemoveLoadingStatus("Loading popular songs..."); 

                PopularSongs = s.Take(50).Select((x, index) => new SongViewModel(x, index + 1)).ToList();
            });
            LoadPopularSongsCommand.ThrownExceptions
                .OfType<GrooveSharkException>()
                .Do(e =>  _loading.RemoveLoadingStatus("Loading popular songs..."))
                .Do(e => Debug.WriteLine("[PopularSongViewModel] : " + e.Description))
                .BindTo(this, self => self.GrooveSharkException);

            #endregion
      
        }


        private List<SongViewModel> _popularSongs;
        public List<SongViewModel> PopularSongs
        {
            get { return _popularSongs; }
            set { this.RaiseAndSetIfChanged(ref _popularSongs, value); }
        }

        public ReactiveCommand<Song[]> LoadPopularSongsCommand { get; set; }

    }
}
