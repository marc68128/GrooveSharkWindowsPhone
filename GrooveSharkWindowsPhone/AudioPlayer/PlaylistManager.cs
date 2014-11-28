using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Playback;
using GrooveSharkWindowsPhone.ViewModels;

namespace GrooveSharkWindowsPhone.AudioPlayer
{
    public class PlaylistManager
    {
        private List<SongViewModel> _playlist;
        private int _current = -1;

        public ISubject<SongViewModel> ActualSongObs;  

        public PlaylistManager()
        {
            _playlist = new List<SongViewModel>();
            ActualSongObs = new Subject<SongViewModel>();
            BackgroundMediaPlayer.Current.MediaEnded +=  OnMediaEnded;
        }

        private void OnMediaEnded(MediaPlayer sender, object args)
        {
            if (_current != _playlist.Count - 1)
                ActualSongObs.OnNext(_playlist[++_current]);
        }

        public void AddSongLast(SongViewModel svm, bool addNext = false, bool play = false)
        {
            if (addNext)
                _playlist.Insert(_current + 1, svm);
            else
                _playlist.Add(svm);

            if (play && addNext)
                ActualSongObs.OnNext(_playlist[++_current]);
            if (play && ! addNext)
            {
                _current = _playlist.Count;
                ActualSongObs.OnNext(_playlist[_current]);
            }
            if (_current == -1)
            {
                _current = 0;
                ActualSongObs.OnNext(_playlist[_current]);
            }                  
        }
    }
}
