using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Playback;
using Windows.UI.Core;
using GrooveSharkClient.Models.Entity;
using GrooveSharkShared;
using GrooveSharkWindowsPhone.AudioPlayer;
using GrooveSharkWindowsPhone.Helpers;
using GrooveSharkWindowsPhone.ViewModels;
using ReactiveUI;

namespace GrooveSharkWindowsPhone
{
    public class AudioPlayerService : ReactiveObject, IAudioPlayerService
    {
        private AutoResetEvent _sererInitialized;
        private int _current = 0;
        private List<SongViewModel> _playList;

        public AudioPlayerService()
        {
            _sererInitialized = new AutoResetEvent(false);
            _playList = new List<SongViewModel>();
            StartBackgroundAudioTask();
        }

        public void AddSongToPlaylist(SongViewModel svm, bool addNext = false, bool play = false)
        {
            if (addNext)
            {
                var valueSet = new ValueSet();
                valueSet.Add(Constants.AddNextSongToPlaylist, svm.Serialize());
                BackgroundMediaPlayer.SendMessageToBackground(valueSet);
                if (!_playList.Any())
                    _playList.Add(svm);
                else
                    _playList.Insert(_current + 1, svm);
            }
            else
            {
                var valueSet = new ValueSet();
                valueSet.Add(Constants.AddSongToPlaylist, svm.Serialize());
                BackgroundMediaPlayer.SendMessageToBackground(valueSet);
                _playList.Add(svm);
            }
            if (play)
            {
                var valueSet = new ValueSet();
                valueSet.Add(Constants.SkipNext, svm.Serialize());
                BackgroundMediaPlayer.SendMessageToBackground(valueSet);
            }
            UpdateActualSongs();
        }

        private SongViewModel _currentSong;
        public SongViewModel CurrentSong
        {
            get { return _currentSong; }
            private set { this.RaiseAndSetIfChanged(ref _currentSong, value); }
        }

        private SongViewModel _nextSong;
        public SongViewModel NextSong
        {
            get { return _nextSong; }
            private set { this.RaiseAndSetIfChanged(ref _nextSong, value); }
        }

        private SongViewModel _previousSong;
        public SongViewModel PreviousSong
        {
            get { return _previousSong; }
            private set { this.RaiseAndSetIfChanged(ref _previousSong, value); }
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
            var backgroundtaskinitializationresult = CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
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
            Debug.WriteLine("BackgroundMediaPlayer_MessageReceivedFromBackground");
            foreach (string key in e.Data.Keys)
            {
                switch (key)
                {
                    case Constants.BackgroundTaskStarted:
                        Debug.WriteLine("Background Task started");
                        _sererInitialized.Set();
                        break;
                    case Constants.CurrentSongChanged:
                        _current = Convert.ToInt32(e.Data[key]);
                        UpdateActualSongs();
                        break;
                }
            }
        }

        private void UpdateActualSongs()
        {
            CurrentSong =_playList[_current];
            NextSong = null;
            PreviousSong = null; 
            if (_playList.Count - 1 > _current)
                NextSong =_playList[_current + 1];
            if (_current > 0)
                PreviousSong = _playList[_current - 1];
        }

        private void MediaPlayer_CurrentStateChanged(MediaPlayer sender, object args)
        {
            Debug.WriteLine("MediaPlayer_CurrentStateChanged");
            Debug.WriteLine(sender.CurrentState);
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
