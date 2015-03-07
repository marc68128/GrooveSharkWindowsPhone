using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Media.Playback;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls.Maps;
using GrooveSharkShared;
using Newtonsoft.Json;
using ReactiveUI;

namespace AudioPlayer
{
    public delegate void SongChangedEventHandler(object sender, SongViewModel svm);

    public sealed class PlaylistManager
    {
        private SystemMediaTransportControls _systemmediatransportcontrol;
        private List<SongViewModel> _playlist;
        private int _current = -1, _timeToSleep;
        private AudioPlayerClient _client;

        public PlaylistManager()
        {
            _playlist = new List<SongViewModel>();
            _client = new AudioPlayerClient();

            BackgroundMediaPlayer.Current.MediaEnded += OnMediaEnded;
            BackgroundMediaPlayer.Current.CurrentStateChanged += OnCurrentStateChanged;

            _systemmediatransportcontrol = SystemMediaTransportControls.GetForCurrentView();
            _systemmediatransportcontrol.ButtonPressed += systemmediatransportcontrol_ButtonPressed;
            _systemmediatransportcontrol.PropertyChanged += systemmediatransportcontrol_PropertyChanged;
            _systemmediatransportcontrol.IsEnabled = true;
            _systemmediatransportcontrol.IsPauseEnabled = true;
            _systemmediatransportcontrol.IsPlayEnabled = true;

            Observable.Interval(new TimeSpan(0, 0, 0, 1)).ObserveOn(RxApp.MainThreadScheduler).Subscribe(_ =>
            {
                if (BackgroundMediaPlayer.Current != null && _current != -1 && _timeToSleep-- <= 0)
                {
                    var position = BackgroundMediaPlayer.Current.Position;
                    if (!_playlist[_current].IsOver30S && position.TotalSeconds > 30)
                    {
                        _client.MarkStreamKeyOver30S(_playlist[_current]);
                        _playlist[_current].IsOver30S = true;
                    }
                    if (_playlist[_current].IsOver30S && position.TotalSeconds > BackgroundMediaPlayer.Current.NaturalDuration.TotalSeconds - 2)
                    {
                        _client.MarkSongComplete(_playlist[_current]);
                        _timeToSleep = 15;
                    }
                }
            });
        }



        private void systemmediatransportcontrol_PropertyChanged(SystemMediaTransportControls sender, SystemMediaTransportControlsPropertyChangedEventArgs args)
        {
            
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

        public void SkipToPrevious()
        {
            if (_current != 0)
            {
                PlaySongAtIndex(_current - 1);
            }
        }
        public void SkipToNext()
        {
            if (_current != _playlist.Count - 1)
            {
                PlaySongAtIndex(_current + 1);
            }
        }

        public string GetSerializedPlaylistInfos()
        {
            if (_current == -1)
            {
                return null;
            }

            SongViewModel[] actualState = new SongViewModel[3];
            actualState[0] = _current == 0 ? new SongViewModel() : _playlist[_current - 1];
            actualState[1] = _playlist[_current];
            actualState[2] = _current + 1 < _playlist.Count ? _playlist[_current + 1] : new SongViewModel();

            return JsonConvert.SerializeObject(actualState);
        }

        private void PlaySongAtIndex(int index)
        {
            _current = index;
            var streamInfoObs = _client.GetStreamInfos(_playlist[_current]).ToObservable();

            streamInfoObs.Subscribe(streamInfo =>
            {
                _playlist[_current].StreamUrl = streamInfo.Url;
                _playlist[_current].StreamServerId = streamInfo.StreamServerID;
                _playlist[_current].StreamKey = streamInfo.StreamKey;
                _playlist[_current].StreamUsecs = streamInfo.Usecs;

                _playlist[_current].IsOver30S = false;

                BackgroundMediaPlayer.Current.SetUriSource(new Uri(_playlist[_current].StreamUrl, UriKind.Absolute));

                var valueSet = new ValueSet();
                valueSet.Add(Constants.PlaylistInfos, GetSerializedPlaylistInfos());
                BackgroundMediaPlayer.SendMessageToForeground(valueSet);

                BackgroundMediaPlayer.Current.Play();
                UpdateSystemMediaTransportControl();
            });
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
        public void AddSong(SongViewModel svm, bool next)
        {
            if (next)
                _playlist.Insert(_current + 1, svm);
            else
                _playlist.Add(svm);

            if (_current == -1)
                PlaySongAtIndex(0);
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
        public string SessionId
        {
            get { return _client.SessionId; }
            set { _client.SessionId = value; }
        }
        public string CountryInfos
        {
            get { return _client.CountryInfos; }
            set { _client.CountryInfos = value; }
        }
    }
}
