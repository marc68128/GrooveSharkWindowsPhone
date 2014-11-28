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
        private SystemMediaTransportControls systemmediatransportcontrol;
        private BackgroundTaskDeferral _deferral;
        private int _current = -1; 
  
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            taskInstance.Canceled += TaskInstance_Canceled;
            BackgroundMediaPlayer.MessageReceivedFromForeground += MessageReceivedFromForeground;
            

            systemmediatransportcontrol = SystemMediaTransportControls.GetForCurrentView();
            systemmediatransportcontrol.ButtonPressed += systemmediatransportcontrol_ButtonPressed;
            systemmediatransportcontrol.PropertyChanged += systemmediatransportcontrol_PropertyChanged;
            systemmediatransportcontrol.IsEnabled = true;
            systemmediatransportcontrol.IsPauseEnabled = true;
            systemmediatransportcontrol.IsPlayEnabled = true;
            systemmediatransportcontrol.IsNextEnabled = true;
            systemmediatransportcontrol.IsPreviousEnabled = true;

            AppSettings.AddValue(Constants.BackgroundTaskState, Constants.BackgroundTaskRunning);
            _deferral = taskInstance.GetDeferral();

            var message = new ValueSet(); 
            message.Add(Constants.BackgroundTaskStarted, "");
            BackgroundMediaPlayer.SendMessageToForeground(message);
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
                }
            }
        }
    }
}
