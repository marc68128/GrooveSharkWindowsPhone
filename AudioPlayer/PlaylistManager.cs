using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using Windows.Media.Playback;

namespace AudioPlayer
{
    public delegate void SongChangedEventHandler(object sender, SongViewModel svm);

    public sealed class PlaylistManager
    {
        private List<SongViewModel> _playlist;
        private int _current = -1;


        public PlaylistManager()
        {
            _playlist = new List<SongViewModel>();
           // _actualSongObs = new Subject<SongViewModel>();
            BackgroundMediaPlayer.Current.MediaEnded +=  OnMediaEnded;
        }

        public event SongChangedEventHandler SongChanged;


        private void OnMediaEnded(MediaPlayer sender, object args)
        {
            //if (_current != _playlist.Count - 1)
               // _actualSongObs.OnNext(_playlist[++_current]);
        }

        public void AddSong(SongViewModel svm)
        {
            _playlist.Add(svm);
            SongChanged.Invoke(this, svm);                  
        }

    }
}
