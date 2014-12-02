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

        }

        private void InitCommands()
        {
            #region AddSongtoUserFavourites

            AddSongToUserFavouritesCommand = ReactiveCommand.CreateAsyncObservable(this.WhenAnyValue(self => self.IsFavorite).Select(x => !x),
                _ => {
                    _loading.AddLoadingStatus("Adding song to your favorites...");
                    return _client.AddSongToUserFavourites(_session.SessionId, SongId);
                });

            AddSongToUserFavouritesCommand.Subscribe(x => {
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
                _ => {
                    _loading.AddLoadingStatus("Removing song from your favorites...");
                    return _client.RemoveUserFavoriteSongs(SongId, _session.SessionId);
                });

            RemoveSongFromUserFavouritesCommand.Subscribe(x => {
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

            AddToPlaylistCommand = ReactiveCommand.Create();
            AddToPlaylistCommand.Subscribe(_ => NavigationHelper.Navigate(typeof(AddSongToPlaylistView), new[] { SongId }));

            #endregion

            #region PlayNexCommand
            PlayNextCommand = ReactiveCommand.Create();
            PlayNextCommand.Subscribe(_ => {
                _audioPlayer.AddSongToPlaylist(this, true);
            });
            #endregion

            #region GetStreamInfoCommand

            GetStreamInfoCommand = ReactiveCommand.CreateAsyncObservable(_ =>
            {
                _loading.AddLoadingStatus("Loading song...");
                return _client.GetStreamInfo(_session.SessionId, _country.Country.Serialize(), SongId);
            });

            GetStreamInfoCommand.WhereNotNull().Subscribe(si =>
            {
                _loading.RemoveLoadingStatus("Loading song...");
                StreamUrl = si.Url;
                StreamKey=  si.StreamKey;
                StreamServerId = si.StreamServerID;
                StreamUsecs = si.Usecs;
            });

            GetStreamInfoCommand.ThrownExceptions
               .OfType<GrooveSharkException>()
               .Do(e => _loading.RemoveLoadingStatus("Loading song..."))
               .Do(e => Debug.WriteLine("[SongViewModel] : " + e.Description))
               .BindTo(this, self => self.GrooveSharkException);

            GetStreamInfoCommand.ThrownExceptions
                .OfType<WebException>()
                .Do(e => _loading.RemoveLoadingStatus("Loading song..."))
                .Do(e => Debug.WriteLine("[SongViewModel] : " + e.Message))
                .BindTo(this, self => self.WebException);

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

        private string _streamUrl;
        public string StreamUrl
        {
            get { return _streamUrl; }
            set { this.RaiseAndSetIfChanged(ref _streamUrl, value); }
        }

        private string _streamKey;
        public string StreamKey
        {
            get { return _streamKey; }
            set { this.RaiseAndSetIfChanged(ref _streamKey, value); }
        }

        private int _streamServerId;
        public int StreamServerId
        {
            get { return _streamServerId; }
            set { this.RaiseAndSetIfChanged(ref _streamServerId, value); }
        }

        private int _streamUsecs;
        public int StreamUsecs
        {
            get { return _streamUsecs; }
            set { this.RaiseAndSetIfChanged(ref _streamUsecs, value); }
        }


        public ReactiveCommand<bool> AddSongToUserFavouritesCommand { get; private set; }
        public ReactiveCommand<bool> RemoveSongFromUserFavouritesCommand { get; private set; }
        public ReactiveCommand<object> AddToPlaylistCommand { get; private set; }
        public ReactiveCommand<object> PlayNextCommand { get; private set; }
        public ReactiveCommand<StreamInfo> GetStreamInfoCommand { get; private set; }

        public static SongViewModel Deserialize(string json)
        {
            var splited = json.Split(';');
            return new SongViewModel() {
                SongName = splited[0],
                SongId = int.Parse(splited[1]),
                AlbumName = splited[2],
                ArtistName = splited[3],
                ThumbnailUrl = splited[4],
                StreamUrl = splited[5],
                StreamKey = splited[6],
                StreamServerId = int.Parse(splited[7]),
                StreamUsecs = int.Parse(splited[8])
            };
        }
        public string Serialize()
        {
            return JsonConvert.SerializeObject(SongName + ";" + SongId + ";" + AlbumName + ";" + ArtistName + ";" + ThumbnailUrl + ";" + StreamUrl + ";" + StreamKey + ";" + StreamServerId + ";" + StreamUsecs);
        }

        
    }

}
