using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Playback;
using Windows.UI.Core;
using GrooveSharkClient.Contracts;
using GrooveSharkShared;
using GrooveSharkWindowsPhone.AudioPlayer;
using GrooveSharkWindowsPhone.Helpers;
using GrooveSharkWindowsPhone.ViewModels;

namespace GrooveSharkWindowsPhone
{
    public class AudioPlayerService : IAudioPlayerService
    {
        private AutoResetEvent _sererInitialized;
        private PlaylistManager _playlistManager;

        public AudioPlayerService()
        {
            _sererInitialized = new AutoResetEvent(false);
            _playlistManager = new PlaylistManager();
            StartBackgroundAudioTask();

            _playlistManager.ActualSongObs.WhereNotNull().Subscribe(svm =>
            {
                svm.GetStreamInfoCommand.Execute(null);
                svm.GetStreamInfoCommand.Subscribe(si =>
                {
                    BackgroundMediaPlayer.Current
                        .SetUriSource(new Uri(si.Url,
                            UriKind.Absolute));
                    BackgroundMediaPlayer.Current.Play();
                });
            });
        }

        public void AddSongToPlaylist(SongViewModel svm, bool addNext = false, bool play = false)
        {
            _playlistManager.AddSongLast(svm, addNext, play);
        }

        public IObservable<SongViewModel> CurrentSongObs { get { return _playlistManager.ActualSongObs; } } 

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
            Debug.WriteLine("BackgroundMediaPlayer_MessageReceivedFromBackground");
            foreach (string key in e.Data.Keys)
            {
                switch (key)
                {
                    case Constants.BackgroundTaskStarted:
                        Debug.WriteLine("Background Task started");
                        _sererInitialized.Set();
                        break;
                }
            }
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
