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

            //IsLoading = true;
            Status = "Loading";

            InitCommands();
        }
        private void InitCommands()
        {
            #region LoadPopularSongsTodayCommand

            LoadPopularSongsTodayCommand =
                ReactiveCommand.CreateAsyncObservable(
                    o => _session.SessionIdObs.SelectMany(session => _client.GetPopularSongToday(session)));
            LoadPopularSongsTodayCommand.Where(s => s != null).Subscribe(s =>
            {
                PopularSongsToday.Clear();
                PopularSongsToday.AddRange(s.Take(50).Select((x, index) => new SongViewModel(x, index + 1)));
                //IsLoading = false;
                Status = "";
            });
            LoadPopularSongsTodayCommand.ThrownExceptions
                .OfType<GrooveSharkException>()
                //.Do(e => IsLoading = false)
                .Do(e => Debug.WriteLine("[GrooveSharkException] : " + e.Description))
                .BindTo(this, self => self.GrooveSharkException);

            #endregion
      
        }

        public ReactiveList<SongViewModel> PopularSongsToday { get; set; }
        public ReactiveCommand<Song[]> LoadPopularSongsTodayCommand { get; set; }


        private GrooveSharkException _grooveSharkException;
        public GrooveSharkException GrooveSharkException
        {
            get { return _grooveSharkException; }
            set { this.RaiseAndSetIfChanged(ref _grooveSharkException, value); }
        }
    }
}
