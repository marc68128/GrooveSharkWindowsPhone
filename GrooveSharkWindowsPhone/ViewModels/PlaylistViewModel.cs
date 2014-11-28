using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GrooveSharkClient.Models;
using GrooveSharkClient.Models.Entity;
using ReactiveUI;

namespace GrooveSharkWindowsPhone.ViewModels
{
    public class PlaylistViewModel : BaseViewModel
    {
        private string _playlistId;
        public PlaylistViewModel(Playlist playlist, bool minimized = false)
        {
            _playlistId = playlist.PlaylistID;
            PlaylistName = playlist.PlaylistName;
            Creator = playlist.FName;

            InitCommand();

            if (!minimized) 
                LoadPlaylistInfoCommand.Execute(null);
        }
        public string PlaylistId { get { return _playlistId; } }

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

            LoadPlaylistSongCommand = ReactiveCommand.CreateAsyncObservable(_ =>
            {
                _loading.AddLoadingStatus("Loading playlist songs...");
                return _client.GetPlaylist(_session.SessionId, _playlistId);
            });

            LoadPlaylistSongCommand.Subscribe(p =>
            {
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

            LoadPlaylistInfoCommand = ReactiveCommand.CreateAsyncObservable(_ =>
            {
                _loading.AddLoadingStatus("Loading playlist Infos...");
                return _client.GetPlaylistInfos(_session.SessionId, _playlistId);
            });

            LoadPlaylistInfoCommand.Subscribe(p =>
            {
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

            ToggleOpenCloseCommand.Subscribe(_ =>
            {
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
        }

        public ReactiveCommand<Playlist> LoadPlaylistSongCommand { get; set; }
        public ReactiveCommand<Playlist> LoadPlaylistInfoCommand { get; set; }
        public ReactiveCommand<object> ToggleOpenCloseCommand { get; set; }

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
