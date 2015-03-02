using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using GrooveSharkClient.Models.Entity;
using GrooveSharkClient.Models.Exception;
using ReactiveUI;

namespace GrooveSharkWindowsPhone.ViewModels
{
    public class SearchViewModel : BaseViewModel
    {
        public SearchViewModel()
        {
            InitCommand();

            this.WhenAnyValue(self => self.SearchResult).Where(x => x != null).Subscribe(r =>
            {
                SongStatus = r.Item1.Any() ? "" : "No Song Matches !";
                SongResult = r.Item1.Select((s, index) => new SongViewModel(s, index + 1)).ToList();

                PlaylistStatus = r.Item2.Any() ? "" : "No Playlist Matches !";
                PlaylistResult = r.Item2.Select(p => new PlaylistViewModel(p, true)).ToList();

                AlbumStatus = r.Item4.Any() ? "" : "No Album Matches !";
                AlbumResult = r.Item4.Select(a => new AlbumViewModel(a)).ToList();

                ArtistStatus = r.Item3.Any() ? "" : "No Artist Matches !";
                ArtistResult = r.Item3.Select(a => new ArtistViewModel(a)).ToList();
            });
        }

        private void InitCommand()
        {
            SearchCommand = ReactiveCommand.CreateAsyncObservable(
                _country.IsDataAvailableObs
                .CombineLatest(_session.IsDataAvailableObs, (b, b1) => b && b1)
                .CombineLatest(this.WhenAnyValue(self => self.SearchTerm).Select(s => !string.IsNullOrEmpty(s)), (b, b1) => b & b1),
                _ =>
                {
                    _loading.AddLoadingStatus("Search...");
                    AppSettings.AddValue("LastSearch", SearchTerm);
                    return _client.SearchAll(SearchTerm, _country.Country.Serialize(), _session.SessionId);
                });

            SearchCommand.Do(_ => _loading.RemoveLoadingStatus("Search...")).BindTo(this, self => self.SearchResult);

            SearchCommand.ThrownExceptions.OfType<WebException>()
              .Do(_ => _loading.RemoveLoadingStatus("Search..."))
              .BindTo(this, self => self.WebException);

            SearchCommand.ThrownExceptions.OfType<GrooveSharkException>()
                .Do(_ => _loading.RemoveLoadingStatus("Search..."))
                .BindTo(this, self => self.GrooveSharkException);
        }

        private Tuple<Song[], Playlist[], Artist[], Album[]> _searchResult;
        public Tuple<Song[], Playlist[], Artist[], Album[]> SearchResult
        {
            get { return _searchResult; }
            set { this.RaiseAndSetIfChanged(ref _searchResult, value); }
        }

        private string _searchTerm;
        public string SearchTerm
        {
            get { return _searchTerm; }
            set { this.RaiseAndSetIfChanged(ref _searchTerm, value); }
        }

        private string _songStatus;
        public string SongStatus
        {
            get { return _songStatus; }
            set { this.RaiseAndSetIfChanged(ref _songStatus, value); }
        }

        private string _playlistStatus;
        public string PlaylistStatus
        {
            get { return _playlistStatus; }
            set { this.RaiseAndSetIfChanged(ref _playlistStatus, value); }
        }

        private string _artistStatus;
        public string ArtistStatus
        {
            get { return _artistStatus; }
            set { this.RaiseAndSetIfChanged(ref _artistStatus, value); }
        }

        private string _albumStatus;
        public string AlbumStatus
        {
            get { return _albumStatus; }
            set { this.RaiseAndSetIfChanged(ref _albumStatus, value); }
        }
        

        private List<SongViewModel> _songResult;
        public List<SongViewModel> SongResult
        {
            get { return _songResult; }
            set { this.RaiseAndSetIfChanged(ref _songResult, value); }
        }

        private List<PlaylistViewModel> _playlistResult;
        public List<PlaylistViewModel> PlaylistResult
        {
            get { return _playlistResult; }
            set { this.RaiseAndSetIfChanged(ref _playlistResult, value); }
        }

        private List<AlbumViewModel> _albumResult;
        public List<AlbumViewModel> AlbumResult
        {
            get { return _albumResult; }
            set { this.RaiseAndSetIfChanged(ref _albumResult, value); }
        }

        private List<ArtistViewModel> _artistResult;
        public List<ArtistViewModel> ArtistResult
        {
            get { return _artistResult; }
            set { this.RaiseAndSetIfChanged(ref _artistResult, value); }
        }


        public ReactiveCommand<Tuple<Song[], Playlist[], Artist[], Album[]>> SearchCommand { get; set; }
    }
}
