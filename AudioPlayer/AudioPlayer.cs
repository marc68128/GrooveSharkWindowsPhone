using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Media.Playback;
using GrooveSharkClient.Models;
using GrooveSharkShared;


namespace AudioPlayer
{
    public sealed class AudioPlayer : IBackgroundTask
    {
        private SystemMediaTransportControls _systemmediatransportcontrol;
        private BackgroundTaskDeferral _deferral;
        private PlaylistManager _playlistManager; 
  
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            taskInstance.Canceled += TaskInstance_Canceled;
            BackgroundMediaPlayer.MessageReceivedFromForeground += MessageReceivedFromForeground;
            
            _playlistManager = new PlaylistManager();
            _playlistManager.SongChanged += PlaylistManagerOnSongChanged;
            

            _systemmediatransportcontrol = SystemMediaTransportControls.GetForCurrentView();
            _systemmediatransportcontrol.ButtonPressed += systemmediatransportcontrol_ButtonPressed;
            _systemmediatransportcontrol.PropertyChanged += systemmediatransportcontrol_PropertyChanged;
            _systemmediatransportcontrol.IsEnabled = true;
            _systemmediatransportcontrol.IsPauseEnabled = true;
            _systemmediatransportcontrol.IsPlayEnabled = true;
            _systemmediatransportcontrol.IsNextEnabled = true;
            _systemmediatransportcontrol.IsPreviousEnabled = true;

            AppSettings.AddValue(Constants.BackgroundTaskState, Constants.BackgroundTaskRunning);
            _deferral = taskInstance.GetDeferral();

            var message = new ValueSet(); 
            message.Add(Constants.BackgroundTaskStarted, "");
            BackgroundMediaPlayer.SendMessageToForeground(message);
        }

        private void PlaylistManagerOnSongChanged(object sender, SongViewModel svm)
        {
            BackgroundMediaPlayer.Current.SetUriSource(new Uri(svm.StreamUrl, UriKind.Absolute));
            BackgroundMediaPlayer.Current.Play();
            _systemmediatransportcontrol.PlaybackStatus = MediaPlaybackStatus.Playing;
            _systemmediatransportcontrol.DisplayUpdater.Type = MediaPlaybackType.Music;
            _systemmediatransportcontrol.DisplayUpdater.MusicProperties.Title = svm.SongName;
            _systemmediatransportcontrol.DisplayUpdater.MusicProperties.Artist = svm.ArtistName;
            _systemmediatransportcontrol.DisplayUpdater.MusicProperties.AlbumArtist = svm.AlbumName;
            _systemmediatransportcontrol.DisplayUpdater.Update();
        }

        private void systemmediatransportcontrol_PropertyChanged(SystemMediaTransportControls sender, SystemMediaTransportControlsPropertyChangedEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void systemmediatransportcontrol_ButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            _deferral.Complete();
        }



        private void MessageReceivedFromForeground(object sender, MediaPlayerDataReceivedEventArgs e)
        {
            foreach (string key in e.Data.Keys)
            {
                switch (key.ToLower())
                {
                    case Constants.Play:
                        Debug.WriteLine(BackgroundMediaPlayer.Current.CurrentState);
                        break;

                    case Constants.AddSongToPlaylist:
                        _playlistManager.AddSong(SongViewModel.Deserialize(e.Data[key].ToString()));
                        break;
                }
            }
        }
    }
}
