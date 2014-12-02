using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reactive.Subjects;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Media.Playback;
using GrooveSharkShared;

namespace AudioPlayer
{
    public delegate void SongChangedEventHandler(object sender, SongViewModel svm);

    public sealed class PlaylistManager
    {
        private SystemMediaTransportControls _systemmediatransportcontrol;
        private List<SongViewModel> _playlist;
        private int _current = -1;

        public PlaylistManager()
        {
            _playlist = new List<SongViewModel>();
            BackgroundMediaPlayer.Current.MediaEnded += OnMediaEnded;
            BackgroundMediaPlayer.Current.CurrentStateChanged += OnCurrentStateChanged;

            _systemmediatransportcontrol = SystemMediaTransportControls.GetForCurrentView();
            _systemmediatransportcontrol.ButtonPressed += systemmediatransportcontrol_ButtonPressed;
            _systemmediatransportcontrol.PropertyChanged += systemmediatransportcontrol_PropertyChanged;
            _systemmediatransportcontrol.IsEnabled = true;
            _systemmediatransportcontrol.IsPauseEnabled = true;
            _systemmediatransportcontrol.IsPlayEnabled = true;
        }



        private void systemmediatransportcontrol_PropertyChanged(SystemMediaTransportControls sender, SystemMediaTransportControlsPropertyChangedEventArgs args)
        {
            throw new NotImplementedException();
        }
        private void systemmediatransportcontrol_ButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            switch (args.Button)
            {
                case SystemMediaTransportControlsButton.Play:
                    Debug.WriteLine("UVC play button pressed");
                    BackgroundMediaPlayer.Current.Play();
                    break;
                case SystemMediaTransportControlsButton.Pause:
                    Debug.WriteLine("UVC pause button pressed");
                    try
                    {
                        BackgroundMediaPlayer.Current.Pause();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }
                    break;
                case SystemMediaTransportControlsButton.Next:
                    Debug.WriteLine("UVC next button pressed");
                    SkipToNext();
                    break;
                case SystemMediaTransportControlsButton.Previous:
                    Debug.WriteLine("UVC previous button pressed");
                    SkipToPrevious();
                    break;
            }
        }

        private void SkipToPrevious()
        {
            if (_current != 0)
            {
                BackgroundMediaPlayer.Current.SetUriSource(new Uri(_playlist[--_current].StreamUrl, UriKind.Absolute));

                var valueSet = new ValueSet();
                valueSet.Add(Constants.CurrentSongChanged, _current);
                BackgroundMediaPlayer.SendMessageToForeground(valueSet);

                BackgroundMediaPlayer.Current.Play();

                UpdateSystemMediaTransportControl();

            }   
        }
        private void SkipToNext()
        {
            if (_current != _playlist.Count - 1)
            {
                BackgroundMediaPlayer.Current.SetUriSource(new Uri(_playlist[++_current].StreamUrl, UriKind.Absolute));

                var valueSet = new ValueSet();
                valueSet.Add(Constants.CurrentSongChanged, _current);
                BackgroundMediaPlayer.SendMessageToForeground(valueSet);

                BackgroundMediaPlayer.Current.Play();

                UpdateSystemMediaTransportControl();

            }
        }

       
        private void OnCurrentStateChanged(MediaPlayer sender, object args)
        {
            var state = BackgroundMediaPlayer.Current.CurrentState;
            switch (state)
            {
                case MediaPlayerState.Playing:
                    _systemmediatransportcontrol.PlaybackStatus = MediaPlaybackStatus.Playing;

                    break;
                case MediaPlayerState.Paused:
                    _systemmediatransportcontrol.PlaybackStatus = MediaPlaybackStatus.Paused;
                    break;
            }
        }
        private void OnMediaEnded(MediaPlayer sender, object args)
        {
            SkipToNext(); 
        }
        public void AddSong(SongViewModel svm)
        {
            _playlist.Add(svm);

            if (_current == -1)
            {
                _current = 0;

                var valueSet = new ValueSet();
                valueSet.Add(Constants.CurrentSongChanged, _current);
                BackgroundMediaPlayer.SendMessageToForeground(valueSet);

                BackgroundMediaPlayer.Current.SetUriSource(new Uri(svm.StreamUrl, UriKind.Absolute));
                BackgroundMediaPlayer.Current.Play();

                _systemmediatransportcontrol.DisplayUpdater.Type = MediaPlaybackType.Music;
                _systemmediatransportcontrol.DisplayUpdater.MusicProperties.Title = svm.SongName;
                _systemmediatransportcontrol.DisplayUpdater.MusicProperties.Artist = svm.ArtistName;
                _systemmediatransportcontrol.DisplayUpdater.MusicProperties.AlbumArtist = svm.AlbumName;
                _systemmediatransportcontrol.DisplayUpdater.Update();
            }
            UpdateSystemMediaTransportControl();
        }

        private void UpdateSystemMediaTransportControl()
        {
            _systemmediatransportcontrol.IsPreviousEnabled = _current > 0;
            _systemmediatransportcontrol.IsNextEnabled = _current < _playlist.Count - 1;

            _systemmediatransportcontrol.DisplayUpdater.Type = MediaPlaybackType.Music;
            _systemmediatransportcontrol.DisplayUpdater.MusicProperties.Title = _playlist[_current].SongName;
            _systemmediatransportcontrol.DisplayUpdater.MusicProperties.Artist = _playlist[_current].ArtistName;
            _systemmediatransportcontrol.DisplayUpdater.MusicProperties.AlbumArtist = _playlist[_current].AlbumName;
            _systemmediatransportcontrol.DisplayUpdater.Update();
        }
    }
}
