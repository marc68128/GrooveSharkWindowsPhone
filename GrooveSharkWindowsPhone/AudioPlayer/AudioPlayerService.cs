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
using Windows.UI.Xaml.Media;
using GrooveSharkClient.Models.Entity;
using GrooveSharkShared;
using GrooveSharkWindowsPhone.AudioPlayer;
using GrooveSharkWindowsPhone.Helpers;
using GrooveSharkWindowsPhone.ViewModels;
using Newtonsoft.Json;
using ReactiveUI;

namespace GrooveSharkWindowsPhone
{
    public class AudioPlayerService : ReactiveObject,  IAudioPlayerService
    {
        private AutoResetEvent _sererInitialized;
        private int _current = 0;

        public AudioPlayerService()
        {
            _sererInitialized = new AutoResetEvent(false);
            StartBackgroundAudioTask();
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
            UpdateActualSongs();
        }
        public void RefreshPlaylist()
        {
            var valueSet = new ValueSet(); 
            valueSet.Add(Constants.PlaylistInfos, null);
            BackgroundMediaPlayer.SendMessageToBackground(valueSet);
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
                    case Constants.PlaylistInfos:
                        var pl = JsonConvert.DeserializeObject<List<string>>(e.Data[key] as string).Select(SongViewModel.Deserialize).ToList();
                        PreviousSong = pl[0].SongId == 0 ? null : pl[0];
                        CurrentSong = pl[1];
                        NextSong = pl[2].SongId == 0 ? null : pl[2];          
                        break;
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
            //CurrentSong =_playList[_current];
            //if (_playList.Count - 1 > _current)
            //    NextSong =_playList[_current + 1];
            //if (_current > 0)
            //    PreviousSong = _playList[_current - 1];
        }

        private void MediaPlayer_CurrentStateChanged(MediaPlayer sender, object args)
        {
            Debug.WriteLine("MediaPlayer_CurrentStateChanged");
            Debug.WriteLine(sender.CurrentState);
            switch (sender.CurrentState)
            {
                case MediaPlayerState.Playing:

                    break;

                case MediaPlayerState.Paused:

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
        }
    }
}
