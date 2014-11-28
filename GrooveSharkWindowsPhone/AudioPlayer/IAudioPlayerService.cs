using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrooveSharkWindowsPhone.ViewModels;

namespace GrooveSharkWindowsPhone.AudioPlayer
{
    public interface IAudioPlayerService
    {
        void AddSongToPlaylist(SongViewModel svm, bool addNext = false, bool play = false);
        IObservable<SongViewModel> CurrentSongObs { get; }
    }
}
