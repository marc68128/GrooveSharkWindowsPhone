using ReactiveUI;

namespace GrooveSharkWindowsPhone.ViewModels
{
    public class PlayerViewModel : BaseViewModel
    {
        public PlayerViewModel()
        {
            Title = "Player";
            _audioPlayer.CurrentSongObs.BindTo(this, self => self.CurrentSong);
        }

        private SongViewModel _currentSong;
        public SongViewModel CurrentSong
        {
            get { return _currentSong; }
            set { this.RaiseAndSetIfChanged(ref _currentSong, value); }
        }

    }
}
