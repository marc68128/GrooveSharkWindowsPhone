using System;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using GrooveSharkClient.Models;
using GrooveSharkClient.Models.Entity;
using GrooveSharkClient.Models.Exception;
using GrooveSharkWindowsPhone.Helpers;
using ReactiveUI;

namespace GrooveSharkWindowsPhone.ViewModels
{
    public class PlaylistViewModel : BaseViewModel
    {
        private int _playlistId;
        public PlaylistViewModel(Playlist playlist, bool minimized = false)
        {
            _playlistId = playlist.PlaylistID;
            PlaylistName = playlist.PlaylistName;
            Creator = playlist.FName;

            InitCommand();

            if (!minimized)
                LoadPlaylistInfoCommand.Execute(null);
        }
        public int PlaylistId { get { return _playlistId; } }

        private string _playlistName;
        public string PlaylistName
        {
            get { return _playlistName; }
            set { this.RaiseAndSetIfChanged(ref _playlistName, value); }
        }

        private string _creator;
        public string Creator
        {
            get { return _creator; }
            set { this.RaiseAndSetIfChanged(ref _creator, value); }
        }

        private string _thumbnailUrl;
        public string ThumbnailUrl
        {
            get { return _thumbnailUrl; }
            set { this.RaiseAndSetIfChanged(ref _thumbnailUrl, value); }
        }

        private Song[] _songs;
        public Song[] Songs
        {
            get { return _songs; }
            set { this.RaiseAndSetIfChanged(ref _songs, value); }
        }

        public void InitCommand()
        {
            #region LoadPlaylistSongCommand

            LoadPlaylistSongCommand = ReactiveCommand.CreateAsyncObservable(_ => {
                _loading.AddLoadingStatus("Loading playlist songs...");
                return _client.GetPlaylist(_session.SessionId, _playlistId);
            });

            LoadPlaylistSongCommand.Subscribe(p => {
                _loading.RemoveLoadingStatus("Loading playlist songs...");
                ThumbnailUrl = "http://images.gs-cdn.net/static/playlists/70_" + p.CoverArtFilename;
                if (string.IsNullOrEmpty(p.CoverArtFilename))
                    ThumbnailUrl = "/Assets/Images/Songs/no_cover_120.png";
                Songs = p.Songs;
            });

            LoadPlaylistSongCommand.ThrownExceptions.OfType<WebException>().BindTo(this, self => self.WebException);
            LoadPlaylistSongCommand.ThrownExceptions.OfType<GrooveSharkException>()
                .BindTo(this, self => self.GrooveSharkException);

            #endregion

            #region LoadPlaylistInfoCommand

            LoadPlaylistInfoCommand = ReactiveCommand.CreateAsyncObservable(_ => {
                _loading.AddLoadingStatus("Loading playlist Infos...");
                return _client.GetPlaylistInfos(_session.SessionId, _playlistId);
            });

            LoadPlaylistInfoCommand.Subscribe(p => {
                _loading.RemoveLoadingStatus("Loading playlist Infos...");


                ThumbnailUrl = "http://images.gs-cdn.net/static/playlists/70_" + p.CoverArtFilename;

                if (string.IsNullOrEmpty(p.CoverArtFilename))
                    ThumbnailUrl = "/Assets/Images/Songs/no_cover_120.png";
                Songs = p.Songs;
            });

            LoadPlaylistInfoCommand.ThrownExceptions.OfType<WebException>().BindTo(this, self => self.WebException);
            LoadPlaylistInfoCommand.ThrownExceptions.OfType<GrooveSharkException>()
                .BindTo(this, self => self.GrooveSharkException);

            #endregion

            #region ToggleOpenCloseCommand

            ToggleOpenCloseCommand = ReactiveCommand.Create();

            ToggleOpenCloseCommand.Subscribe(_ => {
                if (!IsOpen)
                {
                    if (Songs == null || !Songs.Any())
                    {
                        LoadPlaylistSongCommand.Execute(null);
                        LoadPlaylistSongCommand.Take(1).Subscribe(x => IsOpen = true);
                    }
                    else
                        IsOpen = true;
                }
                else
                    IsOpen = false;

            });

            #endregion

            #region PlayNowCommand

            PlayNowCommand = ReactiveCommand.Create(_user.ConnectedUserObs.Select(u => u != null && u.IsAnywhere));

            PlayNowCommand.Subscribe(_ => {
                if (this.Songs == null || !this.Songs.Any())
                {
                    this.WhenAnyValue(self => self.Songs).WhereNotNull().Take(1).Subscribe(s => {
                        _audioPlayer.AddSongToPlaylist(new SongViewModel(s.First(), 0), true, true);
                        foreach (var song in s.Skip(1).Reverse())
                        {
                            _audioPlayer.AddSongToPlaylist(new SongViewModel(song, 0), true, false);
                        }
                    });
                    this.LoadPlaylistSongCommand.Execute(null);
                }
                else
                {
                    _audioPlayer.AddSongToPlaylist(new SongViewModel(Songs.First(), 0), false, true);
                    foreach (var song in Songs.Skip(1).Reverse())
                    {
                        _audioPlayer.AddSongToPlaylist(new SongViewModel(song, 0), true, false);
                    }
                }

            });

            #endregion

            #region PlayNextCommand

            PlayNextCommand = ReactiveCommand.Create(_user.ConnectedUserObs.Select(u => u != null && u.IsAnywhere));

            PlayNextCommand.Subscribe(_ => {
                if (this.Songs == null || !this.Songs.Any())
                {
                    this.WhenAnyValue(self => self.Songs).WhereNotNull().Take(1).Subscribe(s => {
                        foreach (var song in s.Reverse())
                        {
                            _audioPlayer.AddSongToPlaylist(new SongViewModel(song, 0), true, false);
                        }
                    });
                    this.LoadPlaylistSongCommand.Execute(null);
                }
                else
                {
                    foreach (var song in Songs.Reverse())
                    {
                        _audioPlayer.AddSongToPlaylist(new SongViewModel(song, 0), true, false);
                    }
                }

            });

            #endregion

            #region PlayLastCommand

            PlayLastCommand = ReactiveCommand.Create(_user.ConnectedUserObs.Select(u => u != null && u.IsAnywhere));

            PlayLastCommand.Subscribe(_ => {
                if (this.Songs == null || !this.Songs.Any())
                {
                    this.WhenAnyValue(self => self.Songs).WhereNotNull().Take(1).Subscribe(s => {
                        foreach (var song in s)
                        {
                            _audioPlayer.AddSongToPlaylist(new SongViewModel(song, 0), false, false);
                        }
                    });
                    this.LoadPlaylistSongCommand.Execute(null);
                }
                else
                {
                    foreach (var song in Songs)
                    {
                        _audioPlayer.AddSongToPlaylist(new SongViewModel(song, 0), false, false);
                    }
                }

            });

            #endregion
        }

        public ReactiveCommand<Playlist> LoadPlaylistSongCommand { get; private set; }
        public ReactiveCommand<Playlist> LoadPlaylistInfoCommand { get; private set; }
        public ReactiveCommand<object> ToggleOpenCloseCommand { get; private set; }
        public ReactiveCommand<object> PlayNowCommand { get; private set; }
        public ReactiveCommand<object> PlayNextCommand { get; private set; }
        public ReactiveCommand<object> PlayLastCommand { get; private set; }

        private bool _isOpen;
        public bool IsOpen
        {
            get { return _isOpen; }
            set { this.RaiseAndSetIfChanged(ref _isOpen, value); }
        }


        public override string ToString()
        {
            return "Playlist : " + PlaylistName;
        }
    }
}
