using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Playback;
using Windows.UI.Core;
using GrooveSharkShared;
using GrooveSharkWindowsPhone.AudioPlayer;
using GrooveSharkWindowsPhone.Helpers;
using GrooveSharkWindowsPhone.ViewModels;

namespace GrooveSharkWindowsPhone
{
    public class AudioPlayerService : IAudioPlayerService
    {
        private AutoResetEvent _sererInitialized;

        public AudioPlayerService()
        {
            _sererInitialized = new AutoResetEvent(false);
            StartBackgroundAudioTask();


        }

        public void AddSongToPlaylist(SongViewModel svm, bool addNext = false, bool play = false)
        {
            svm.GetStreamInfoCommand.Execute(null);
            svm.GetStreamInfoCommand.Take(1).Subscribe(_ => {
                var valueSet = new ValueSet();
                valueSet.Add(Constants.AddSongToPlaylist, svm.Serialize());
                BackgroundMediaPlayer.SendMessageToBackground(valueSet);
            });

        }

        public IObservable<SongViewModel> CurrentSongObs { get; private set; }


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
