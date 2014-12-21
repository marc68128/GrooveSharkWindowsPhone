
using System;
using System.Reactive.Linq;
using Windows.Foundation.Collections;
using Windows.Media.Playback;
using GrooveSharkShared;
using ReactiveUI;

namespace GrooveSharkWindowsPhone.ViewModels
{
    public class PlayerViewModel : BaseViewModel
    {
        public PlayerViewModel()
        {
            Title = "Player";
            SetupBindings();
            SetupCommands();
        }

        private void SetupBindings()
        {
            _audioPlayer.WhenAnyValue(p => p.CurrentSong).ObserveOn(RxApp.MainThreadScheduler).BindTo(this, self => self.CurrentSong);
            _audioPlayer.WhenAnyValue(p => p.NextSong).ObserveOn(RxApp.MainThreadScheduler).BindTo(this, self => self.NextSong);
            _audioPlayer.WhenAnyValue(p => p.PreviousSong).ObserveOn(RxApp.MainThreadScheduler).BindTo(this, self => self.PreviousSong);
            _audioPlayer.WhenAnyValue(p => p.IsPlaying).ObserveOn(RxApp.MainThreadScheduler).BindTo(this, self => self.IsPlaying);
        }

        private void SetupCommands()
        {
            SkipNextCommand = ReactiveCommand.Create(this.WhenAnyValue(self => self.NextSong).Select(n => n != null));
            SkipNextCommand.Subscribe(_ => BackgroundMediaPlayer.SendMessageToBackground(new ValueSet { { Constants.SkipNext, "" } }));

            SkipPreviousCommand = ReactiveCommand.Create(this.WhenAnyValue(self => self.PreviousSong).Select(n => n != null));
            SkipPreviousCommand.Subscribe(_ => BackgroundMediaPlayer.SendMessageToBackground(new ValueSet { { Constants.SkipPrevious, "" } }));

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

        private bool _isPlaying;
        public bool IsPlaying
        {
            get { return _isPlaying; }
            private set { this.RaiseAndSetIfChanged(ref _isPlaying, value); }
        }

        public ReactiveCommand<object> SkipNextCommand { get; private set; }
        public ReactiveCommand<object> SkipPreviousCommand { get; private set; }
        public ReactiveCommand<object> TogglePlayPauseCommand { get; private set; }

    }
}
