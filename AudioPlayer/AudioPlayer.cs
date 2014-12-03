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
using GrooveSharkShared;


namespace AudioPlayer
{
    public sealed class AudioPlayer : IBackgroundTask
    {
        private BackgroundTaskDeferral _deferral;
        private PlaylistManager _playlistManager;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            taskInstance.Canceled += TaskCanceled;
            taskInstance.Task.Completed += TaskCompleted;
            AppSettings.AddValue(Constants.IsBackgroundTaskRunning, true);

            BackgroundMediaPlayer.MessageReceivedFromForeground += MessageReceivedFromForeground;

            _playlistManager = new PlaylistManager();

            _deferral = taskInstance.GetDeferral();

            var message = new ValueSet();
            message.Add(Constants.BackgroundTaskStarted, "");
            BackgroundMediaPlayer.SendMessageToForeground(message);
        }



        void TaskCompleted(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
        {
            Debug.WriteLine("MyBackgroundAudioTask " + sender.TaskId + " Completed...");
            AppSettings.AddValue(Constants.IsBackgroundTaskRunning, false);
            _deferral.Complete();
        }
        private void TaskCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            Debug.WriteLine("MyBackgroundAudioTask Canceled... Reason : " + reason);
            AppSettings.AddValue(Constants.IsBackgroundTaskRunning, false);
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
                    case Constants.SessionIdChanged:
                        _playlistManager.SessionId = e.Data[key].ToString();
                        Debug.WriteLine("[AudioPlayer] SessionIdChanged : " + _playlistManager.SessionId);
                        break;
                    case Constants.CountryInfosChanged:
                        _playlistManager.CountryInfos = e.Data[key].ToString();
                        Debug.WriteLine("[AudioPlayer] CountryInfosChanged : " + _playlistManager.CountryInfos);
                        break;
                    case Constants.AddSongToPlaylist:
                        _playlistManager.AddSong(SongViewModel.Deserialize(e.Data[key].ToString()), false);
                        break;
                    case Constants.AddNextSongToPlaylist:
                        _playlistManager.AddSong(SongViewModel.Deserialize(e.Data[key].ToString()), true);
                        break;
                    case Constants.SkipNext:
                        _playlistManager.SkipToNext();
                        break;
                    case Constants.SkipPrevious:
                        _playlistManager.SkipToPrevious();
                        break;
                }
            }
        }
    }
}
