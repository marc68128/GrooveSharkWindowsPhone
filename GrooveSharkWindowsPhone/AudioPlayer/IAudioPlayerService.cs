using System;
using System.Reactive.Subjects;
using GrooveSharkWindowsPhone.ViewModels;
using ReactiveUI;

namespace GrooveSharkWindowsPhone.AudioPlayer
{
    public interface IAudioPlayerService
    {
        void AddSongToPlaylist(SongViewModel svm, bool addNext = false, bool play = false);
        void RefreshPlaylist();
        SongViewModel CurrentSong { get; }
        SongViewModel NextSong { get; } 
        SongViewModel PreviousSong { get; }

        ReactiveList<SongViewModel> Playlist { get; }
        int CurrentIndex { get; }

        bool IsPlaying { get; }
    }
}
