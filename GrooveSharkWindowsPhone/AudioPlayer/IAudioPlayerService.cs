using System;
using System.Reactive.Subjects;
using GrooveSharkWindowsPhone.ViewModels;

namespace GrooveSharkWindowsPhone.AudioPlayer
{
    public interface IAudioPlayerService
    {
        void AddSongToPlaylist(SongViewModel svm, bool addNext = false, bool play = false);
        SongViewModel CurrentSong { get; }
        SongViewModel NextSong { get; } 
        SongViewModel PreviousSong { get; }

        bool IsPlaying { get; }
    }
}
