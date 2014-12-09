using System;
using System.Diagnostics;
using System.Net;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using Windows.UI.Popups;
using GrooveSharkClient.Models.Entity;
using GrooveSharkClient.Models.Exception;
using GrooveSharkWindowsPhone.Helpers;
using GrooveSharkWindowsPhone.Views;
using Newtonsoft.Json;
using ReactiveUI;

namespace GrooveSharkWindowsPhone.ViewModels
{
    [DataContract]
    public class SongViewModel : BaseViewModel
    {
        public SongViewModel(Song s, int position, bool isFavorite = false)
        {
            _user.ConnectedUserObs.BindTo(this, self => self.CurrentUser);
            IsFavorite = isFavorite;
            SongPosition = position;
            SongName = s.SongName;
            ArtistName = s.ArtistName;
            AlbumName = s.AlbumName;
            SongId = s.SongID;
            ThumbnailUrl = (!string.IsNullOrEmpty(s.CoverArtFilename) && s.CoverArtFilename != "0") ? "http://images.gs-cdn.net/static/albums/70_" + s.CoverArtFilename : "/Assets/Images/Songs/no_cover_63.png";
            InitCommands();
        }
        public SongViewModel()
        {
            _user.ConnectedUserObs.BindTo(this, self => self.CurrentUser); 
        }

        private void InitCommands()
        {
            #region AddSongtoUserFavourites

            AddSongToUserFavouritesCommand = ReactiveCommand.CreateAsyncObservable(this.WhenAnyValue(self => self.IsFavorite).Select(x => !x).CombineLatest(_user.ConnectedUserObs.Select(u => u != null && u.UserID != 0), (a, b) => a && b),
                _ =>
                {
                    _loading.AddLoadingStatus("Adding song to your favorites...");
                    return _client.AddSongToUserFavourites(_session.SessionId, SongId);
                });

            AddSongToUserFavouritesCommand.Subscribe(x =>
            {
                IsFavorite = true;
                _loading.RemoveLoadingStatus("Adding song to your favorites...");
                new MessageDialog(SongName + " added to your favourites.").ShowAsync();
            });

            AddSongToUserFavouritesCommand.ThrownExceptions.OfType<WebException>()
                .Do(_ => _loading.RemoveLoadingStatus("Adding song to your favorites..."))
                .BindTo(this, self => self.WebException);

            AddSongToUserFavouritesCommand.ThrownExceptions.OfType<GrooveSharkException>()
                .Do(_ => _loading.RemoveLoadingStatus("Adding song to your favorites..."))
                .BindTo(this, self => self.GrooveSharkException);

            #endregion

            #region RemoveSongFromUserFavourites

            RemoveSongFromUserFavouritesCommand = ReactiveCommand.CreateAsyncObservable(this.WhenAnyValue(self => self.IsFavorite),
                _ =>
                {
                    _loading.AddLoadingStatus("Removing song from your favorites...");
                    return _client.RemoveUserFavoriteSongs(SongId, _session.SessionId);
                });

            RemoveSongFromUserFavouritesCommand.Subscribe(x =>
            {
                IsFavorite = false;
                _loading.RemoveLoadingStatus("Removing song from your favorites...");
                new MessageDialog(SongName + " removed from your favourites.").ShowAsync();
            });

            RemoveSongFromUserFavouritesCommand.ThrownExceptions.OfType<WebException>()
                .Do(_ => _loading.RemoveLoadingStatus("Removing song from your favorites..."))
                .BindTo(this, self => self.WebException);

            RemoveSongFromUserFavouritesCommand.ThrownExceptions.OfType<GrooveSharkException>()
                .Do(_ => _loading.RemoveLoadingStatus("Removing song from your favorites..."))
                .BindTo(this, self => self.GrooveSharkException);

            #endregion

            #region AddToPlaylistCommand

            AddToPlaylistCommand = ReactiveCommand.Create(_user.ConnectedUserObs.Select(u => u != null && u.UserID != 0));
            AddToPlaylistCommand.Subscribe(_ => NavigationHelper.Navigate(typeof(AddSongToPlaylistView), new[] { SongId }));

            #endregion

            #region PlayNextCommand

            PlayNextCommand = ReactiveCommand.Create(_user.ConnectedUserObs.Select(u => u != null && u.IsAnywhere));
            PlayNextCommand.Subscribe(_ => _audioPlayer.AddSongToPlaylist(this, true));

            #endregion

            #region PlayNowCommand

            PlayNowCommand = ReactiveCommand.Create(_user.ConnectedUserObs.Select(u => u != null && u.IsAnywhere));
            PlayNowCommand.Subscribe(_ => _audioPlayer.AddSongToPlaylist(this, true, true));

            #endregion

            #region PlayLastCommand

            PlayLastCommand = ReactiveCommand.Create(_user.ConnectedUserObs.Select(u => u != null && u.IsAnywhere));
            PlayLastCommand.Subscribe(_ => _audioPlayer.AddSongToPlaylist(this));

            #endregion
        }

        private int _songId;
        public int SongId
        {
            get { return _songId; }
            set { this.RaiseAndSetIfChanged(ref _songId, value); }
        }

        private string _songName;
        public string SongName
        {
            get { return _songName; }
            set { this.RaiseAndSetIfChanged(ref _songName, value); }
        }

        private string _artistName;
        public string ArtistName
        {
            get { return _artistName; }
            set { this.RaiseAndSetIfChanged(ref _artistName, value); }
        }

        private string _albumName;
        public string AlbumName
        {
            get { return _albumName; }
            set { this.RaiseAndSetIfChanged(ref _albumName, value); }
        }

        private int _songPosition;
        public int SongPosition
        {
            get { return _songPosition; }
            set { this.RaiseAndSetIfChanged(ref _songPosition, value); }
        }

        private string _thumbnailUrl;
        public string ThumbnailUrl
        {
            get { return _thumbnailUrl; }
            set { this.RaiseAndSetIfChanged(ref _thumbnailUrl, value); }
        }

        private bool _isFavorite;
        public bool IsFavorite
        {
            get { return _isFavorite; }
            set { this.RaiseAndSetIfChanged(ref _isFavorite, value); }
        }

        public User CurrentUser { get; set; }


        public ReactiveCommand<bool> AddSongToUserFavouritesCommand { get; private set; }
        public ReactiveCommand<bool> RemoveSongFromUserFavouritesCommand { get; private set; }
        public ReactiveCommand<object> AddToPlaylistCommand { get; private set; }
        public ReactiveCommand<object> PlayNextCommand { get; private set; }
        public ReactiveCommand<object> PlayNowCommand { get; private set; }
        public ReactiveCommand<object> PlayLastCommand { get; private set; }

        public static SongViewModel Deserialize(string json)
        {
            var splited = json.Replace("\"\\\"", "").Replace("\\\"\"", "").Split(';');
            return new SongViewModel()
            {
                SongName = splited[0],
                SongId = int.Parse(splited[1]),
                AlbumName = splited[2],
                ArtistName = splited[3],
                ThumbnailUrl = splited[4]
            };
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(SongName + ";" + SongId + ";" + AlbumName + ";" + ArtistName + ";" + ThumbnailUrl);
        }


    }

}
