using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using AudioPlayer;
using GrooveSharkClient.Models;
using GrooveSharkClient.Models.Entity;
using GrooveSharkWindowsPhone.Helpers;
using GrooveSharkWindowsPhone.Views;
using ReactiveUI;

namespace GrooveSharkWindowsPhone.ViewModels
{
    public class SongViewModel : BaseViewModel
    {
        private Song _s;

        public SongViewModel(Song s, int position, bool isFavorite = false)
        {
            _s = s;
            IsFavorite = isFavorite;
            SongPosition = position;
            ThumbnailUrl = (!string.IsNullOrEmpty(s.CoverArtFilename) && s.CoverArtFilename != "0") ? "http://images.gs-cdn.net/static/albums/70_" + s.CoverArtFilename : "/Assets/Images/Songs/no_cover_63.png";
            InitCommands();
        }

        private void InitCommands()
        {
            #region AddSongtoUserFavourites

            AddSongToUserFavouritesCommand = ReactiveCommand.CreateAsyncObservable(this.WhenAnyValue(self => self.IsFavorite).Select(x => !x),
                _ =>
                {
                    _loading.AddLoadingStatus("Adding song to your favorites...");
                    return _client.AddSongToUserFavourites(_session.SessionId, _s.SongID.ToString());
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
                    return _client.RemoveUserFavoriteSongs(_s.SongID.ToString(), _session.SessionId);
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

            AddToPlaylistCommand = ReactiveCommand.Create();
            AddToPlaylistCommand.Subscribe(_ => NavigationHelper.Navigate(typeof(AddSongToPlaylistView), new[] { _s.SongID }));

            #endregion

            #region PlayNexCommand
            PlayNextCommand = ReactiveCommand.Create();
            PlayNextCommand.Subscribe(_ =>
            {
                _audioPlayer.AddSongToPlaylist(this, true);
            });
            #endregion

            #region GetStreamInfoCommand

            GetStreamInfoCommand = ReactiveCommand.CreateAsyncObservable(_ => _client.GetStreamInfo(_session.SessionId,
                                         _country.Country.GetCountryInfoAsJsonString(), _s.SongID.ToString()));

            #endregion
        }

        public string ArtistName { get { return _s.ArtistName; } }
        public string SongName { get { return _s.SongName; } }
        public string AlbumName { get { return _s.AlbumName; } }

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


        public ReactiveCommand<bool> AddSongToUserFavouritesCommand { get; private set; }
        public ReactiveCommand<bool> RemoveSongFromUserFavouritesCommand { get; private set; }
        public ReactiveCommand<object> AddToPlaylistCommand { get; private set; }
        public ReactiveCommand<object> PlayNextCommand { get; private set; }
        public ReactiveCommand<StreamInfo> GetStreamInfoCommand { get; private set; }
    }

}
