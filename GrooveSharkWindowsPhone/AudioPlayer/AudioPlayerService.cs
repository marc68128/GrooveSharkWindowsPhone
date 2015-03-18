using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Playback;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using GrooveSharkClient.Contracts;
using GrooveSharkClient.Models.Entity;
using GrooveSharkShared;
using GrooveSharkWindowsPhone.AudioPlayer;
using GrooveSharkWindowsPhone.Helpers;
using GrooveSharkWindowsPhone.ViewModels;
using Newtonsoft.Json;
using ReactiveUI;
using Splat;

namespace GrooveSharkWindowsPhone
{
    public class AudioPlayerService : ReactiveObject, IAudioPlayerService
    {
        private AutoResetEvent _sererInitialized;
        private ILoadingService _loadingService;
        private MediaPlayerState _lastState;
        private MediaPlayerState _currentState;

        public AudioPlayerService()
        {
            _sererInitialized = new AutoResetEvent(false);
            Playlist = new ReactiveList<SongViewModel>();
            StartBackgroundAudioTask();
            _loadingService = Locator.Current.GetService<ILoadingService>();
        }

        public void AddSongToPlaylist(SongViewModel svm, bool addNext = false, bool play = false)
        {
            if (addNext)
            {
                var valueSet = new ValueSet();
                valueSet.Add(Constants.AddNextSongToPlaylist, svm.Serialize());
                BackgroundMediaPlayer.SendMessageToBackground(valueSet);
            }
            else
            {
                var valueSet = new ValueSet();
                valueSet.Add(Constants.AddSongToPlaylist, svm.Serialize());
                BackgroundMediaPlayer.SendMessageToBackground(valueSet);
            }
            if (play)
            {
                var valueSet = new ValueSet();
                valueSet.Add(Constants.SkipNext, null);
                BackgroundMediaPlayer.SendMessageToBackground(valueSet);
            }
        }
        public void RefreshPlaylist()
        {
            var valueSet = new ValueSet();
            valueSet.Add(Constants.PlaylistInfos, null);
            BackgroundMediaPlayer.SendMessageToBackground(valueSet);
        }

        private void RefreshPlaylist(string jsonPlaylistInfo)
        {
            var pl = JsonConvert.DeserializeObject<Tuple<List<SongViewModel>, int>>(jsonPlaylistInfo);
            Playlist.Clear();
            Playlist.AddRange(pl.Item1);
            CurrentIndex = pl.Item2;
            if (CurrentIndex >= 0)
                CurrentSong = Playlist[CurrentIndex];
            if (CurrentIndex >= 1)
                PreviousSong = Playlist[CurrentIndex - 1];
            if (CurrentIndex + 1 <= Playlist.Count - 1)
                NextSong = Playlist[CurrentIndex + 1];

        }

        public ReactiveList<SongViewModel> Playlist { get; private set; }

        private int _currentIndex;
        public int CurrentIndex
        {
            get { return _currentIndex; }
            private set { this.RaiseAndSetIfChanged(ref _currentIndex, value); }
        }

        private SongViewModel _currentSong;
        public SongViewModel CurrentSong
        {
            get { return _currentSong; }
            private set { this.RaiseAndSetIfChanged(ref _currentSong, value); }
        }

        private SongViewModel _previousSong;
        public SongViewModel PreviousSong
        {
            get { return _previousSong; }
            private set { this.RaiseAndSetIfChanged(ref _previousSong, value); }
        }

        private SongViewModel _nextSong;
        public SongViewModel NextSong
        {
            get { return _nextSong; }
            private set { this.RaiseAndSetIfChanged(ref _nextSong, value); }
        }


        private bool _isPlaying;
        public bool IsPlaying
        {
            get { return _isPlaying; }
            private set { this.RaiseAndSetIfChanged(ref _isPlaying, value); }
        }

        private void StartBackgroundAudioTask()
        {
            AddMediaPlayerEventHandlers();
            var backgroundtaskinitializationresult = CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                bool result = _sererInitialized.WaitOne(2000);
                if (!result)
                {
                    //throw new Exception("Background Audio Task didn't start in expected time");
                }
            }
            );
            backgroundtaskinitializationresult.Completed = BackgroundTaskInitializationCompleted;
        }

        private void AddMediaPlayerEventHandlers()
        {
            BackgroundMediaPlayer.MessageReceivedFromBackground += this.BackgroundMediaPlayer_MessageReceivedFromBackground;
            BackgroundMediaPlayer.Current.CurrentStateChanged += this.MediaPlayer_CurrentStateChanged;
        }

        private void BackgroundMediaPlayer_MessageReceivedFromBackground(object sender, MediaPlayerDataReceivedEventArgs e)
        {
            foreach (string key in e.Data.Keys)
            {
                switch (key)
                {
                    case Constants.PlaylistInfos:
                        if (e.Data[key] == null)
                            break;
                        RefreshPlaylist(e.Data[key] as string);
                        break;
                    case Constants.BackgroundTaskStarted:
                        Debug.WriteLine("Background Task started");
                        _sererInitialized.Set();
                        break;

                }
            }
        }

        private async void MediaPlayer_CurrentStateChanged(MediaPlayer sender, object args)
        {
            _currentState = BackgroundMediaPlayer.Current.CurrentState;
            Debug.WriteLine(_currentState + " !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            if (_lastState != _currentState)
            {
                if (_lastState == MediaPlayerState.Buffering || _lastState == MediaPlayerState.Opening)
                {
                    _loadingService = _loadingService ?? Locator.Current.GetService<ILoadingService>(); 
                    _loadingService.RemoveLoadingStatus(_lastState + " song...");
                }
                if (_currentState == MediaPlayerState.Buffering || _currentState == MediaPlayerState.Opening)
                {
                    _loadingService = _loadingService ?? Locator.Current.GetService<ILoadingService>();
                    _loadingService.AddLoadingStatus(_currentState + " song...");
                }
                _lastState = BackgroundMediaPlayer.Current.CurrentState;
            }
            switch (sender.CurrentState)
            {
                case MediaPlayerState.Playing:
                    IsPlaying = true;
                    break;

                case MediaPlayerState.Paused:
                    IsPlaying = false;
                    break;
            }
        }

        private void BackgroundTaskInitializationCompleted(IAsyncAction asyncinfo, AsyncStatus asyncstatus)
        {
            if (asyncstatus == AsyncStatus.Completed)
            {
                Debug.WriteLine("Background Audio Task initialized");
            }
            else if (asyncstatus == AsyncStatus.Error)
            {
                Debug.WriteLine("Background Audio Task could not initialized due to an error ::" + asyncinfo.ErrorCode.ToString());
            }
            IsPlaying = BackgroundMediaPlayer.IsMediaPlaying();
        }

    }
}
