using System.Reactive.Linq;
using ReactiveUI;

namespace GrooveSharkWindowsPhone.ViewModels
{
    public class PlayerViewModel : BaseViewModel
    {
        public PlayerViewModel()
        {
            Title = "Player";
            _audioPlayer.WhenAnyValue(p => p.CurrentSong).ObserveOn(RxApp.MainThreadScheduler).BindTo(this, self => self.CurrentSong);
            _audioPlayer.WhenAnyValue(p => p.NextSong).ObserveOn(RxApp.MainThreadScheduler).BindTo(this, self => self.NextSong);
            _audioPlayer.WhenAnyValue(p => p.PreviousSong).ObserveOn(RxApp.MainThreadScheduler).BindTo(this, self => self.PreviousSong);
            _audioPlayer.RefreshPlaylist();
        }

        private SongViewModel _currentSong;
        public SongViewModel CurrentSong
        {
            get { return _currentSong; }
            set { this.RaiseAndSetIfChanged(ref _currentSong, value); }
        }

        private SongViewModel _nextSong;
        public SongViewModel NextSong
        {
            get { return _nextSong; }
            set { this.RaiseAndSetIfChanged(ref _nextSong, value); }
        }

        private SongViewModel _previousSong;
        public SongViewModel PreviousSong
        {
            get { return _previousSong; }
            set { this.RaiseAndSetIfChanged(ref _previousSong, value); }
        }
    }
}
