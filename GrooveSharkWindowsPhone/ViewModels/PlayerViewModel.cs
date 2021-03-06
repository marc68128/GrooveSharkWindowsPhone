
using System;
using System.Diagnostics;
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
            _audioPlayer.RefreshPlaylist();
            SetupBindings();
            SetupCommands();
        }

        private void SetupBindings()
        {
            _audioPlayer.WhenAnyValue(p => p.CurrentSong).ObserveOn(RxApp.MainThreadScheduler).BindTo(this, self => self.CurrentSong);
            _audioPlayer.WhenAnyValue(p => p.NextSong).ObserveOn(RxApp.MainThreadScheduler).BindTo(this, self => self.NextSong);
            _audioPlayer.WhenAnyValue(p => p.PreviousSong).ObserveOn(RxApp.MainThreadScheduler).BindTo(this, self => self.PreviousSong);
            _audioPlayer.WhenAnyValue(p => p.IsPlaying).ObserveOn(RxApp.MainThreadScheduler).BindTo(this, self => self.IsPlaying);
            _audioPlayer.WhenAnyValue(p => p.IsPlaying).ObserveOn(RxApp.MainThreadScheduler).Select(b => new Uri(b ? "ms-appx:/Assets/Icons/pause.png" : "ms-appx:/Assets/Icons/play.png", UriKind.RelativeOrAbsolute)).BindTo(this, self => self.TogglePlayPauseThumbnailUrl);
            
            Observable.Interval(new TimeSpan(0, 0, 0, 0, 200)).ObserveOn(RxApp.MainThreadScheduler).Subscribe(_ =>
            {
                if (BackgroundMediaPlayer.Current != null)
                {
                    CurrentSongDuration = BackgroundMediaPlayer.Current.NaturalDuration;
                    Position = BackgroundMediaPlayer.Current.Position;
                    if (BackgroundMediaPlayer.Current.NaturalDuration.Ticks != 0)
                        Progress = (100 * BackgroundMediaPlayer.Current.Position.TotalSeconds) / BackgroundMediaPlayer.Current.NaturalDuration.TotalSeconds;
                }
            });
        }
        private void SetupCommands()
        {
            SkipNextCommand = ReactiveCommand.Create(this.WhenAnyValue(self => self.NextSong).Select(n => n != null));
            SkipNextCommand.Subscribe(_ => BackgroundMediaPlayer.SendMessageToBackground(new ValueSet { { Constants.SkipNext, "" } }));

            SkipPreviousCommand = ReactiveCommand.Create(this.WhenAnyValue(self => self.PreviousSong).Select(n => n != null));
            SkipPreviousCommand.Subscribe(_ => BackgroundMediaPlayer.SendMessageToBackground(new ValueSet { { Constants.SkipPrevious, "" } }));

            TogglePlayPauseCommand = ReactiveCommand.Create();
            TogglePlayPauseCommand.Subscribe(_ =>
            {
                if (IsPlaying)
                    BackgroundMediaPlayer.Current.Pause();
                else
                    BackgroundMediaPlayer.Current.Play();
            });

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

        private Uri _togglePlayPauseThumbnailUrl;
        public Uri TogglePlayPauseThumbnailUrl
        {
            get { return _togglePlayPauseThumbnailUrl; }
            set { this.RaiseAndSetIfChanged(ref _togglePlayPauseThumbnailUrl, value); }
        }

        private double _progress;
        public double Progress
        {
            get { return _progress; }
            set { this.RaiseAndSetIfChanged(ref _progress, value); }
        }

        private TimeSpan _position;
        public TimeSpan Position
        {
            get { return _position; }
            set { this.RaiseAndSetIfChanged(ref _position, value); }
        }

        private TimeSpan _currentSongDuration;
        public TimeSpan CurrentSongDuration
        {
            get { return _currentSongDuration; }
            set { this.RaiseAndSetIfChanged(ref _currentSongDuration, value); }
        }
        

        public ReactiveCommand<object> SkipNextCommand { get; private set; }
        public ReactiveCommand<object> SkipPreviousCommand { get; private set; }
        public ReactiveCommand<object> TogglePlayPauseCommand { get; private set; }

    }
}
