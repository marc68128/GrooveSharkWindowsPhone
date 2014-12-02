using System;
using GrooveSharkWindowsPhone.ViewModels;

namespace GrooveSharkWindowsPhone.AudioPlayer
{
    public interface IAudioPlayerService
    {
        void AddSongToPlaylist(SongViewModel svm, bool addNext = false, bool play = false);
        IObservable<SongViewModel> CurrentSongObs { get; }
    }
}
