using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using GrooveSharkClient.Models.Entity;
using GrooveSharkClient.Models.Exception;
using ReactiveUI;

namespace GrooveSharkWindowsPhone.ViewModels
{
    public class AlbumViewModel : BaseViewModel
    {
        private int _albumID;
        public AlbumViewModel(Album album)
        {
            _albumID = album.AlbumID;
            AlbumName = album.AlbumName;
            ThumbnailUrl = "http://images.gs-cdn.net/static/albums/70_" + album.CoverArtFilename;
            if (string.IsNullOrEmpty(album.CoverArtFilename))
                ThumbnailUrl = "/Assets/Images/Songs/no_cover_120.png";
            InitCommand();
        }
        public int AlbumId { get { return _albumID; } }

        private string _albumName;
        public string AlbumName
        {
            get { return _albumName; }
            set { this.RaiseAndSetIfChanged(ref _albumName, value); }
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
            #region LoadAlbumSongCommand

            LoadAlbumSongCommand = ReactiveCommand.CreateAsyncObservable(_ =>
            {
                _loading.AddLoadingStatus("Loading album songs...");
                return _client.GetAlbumSongs(_session.SessionId, _albumID);
            });

            LoadAlbumSongCommand.Subscribe(p =>
            {
                _loading.RemoveLoadingStatus("Loading album songs...");
                Songs = p;
            });

            LoadAlbumSongCommand.ThrownExceptions.OfType<WebException>().BindTo(this, self => self.WebException);
            LoadAlbumSongCommand.ThrownExceptions.OfType<GrooveSharkException>()
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
                        LoadAlbumSongCommand.Execute(null);
                        LoadAlbumSongCommand.Take(1).Subscribe(x => IsOpen = true);
                    }
                    else
                        IsOpen = true;
                }
                else
                    IsOpen = false;

            });

            #endregion
        }

        public ReactiveCommand<Song[]> LoadAlbumSongCommand { get; set; }
        public ReactiveCommand<object> ToggleOpenCloseCommand { get; set; }

        private bool _isOpen;
        public bool IsOpen
        {
            get { return _isOpen; }
            set { this.RaiseAndSetIfChanged(ref _isOpen, value); }
        }


        public override string ToString()
        {
            return "Album : " + AlbumName;
        }
    }
}
