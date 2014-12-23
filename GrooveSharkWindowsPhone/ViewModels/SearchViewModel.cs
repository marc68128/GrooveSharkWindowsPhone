﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using GrooveSharkClient.Models.Entity;
using ReactiveUI;

namespace GrooveSharkWindowsPhone.ViewModels
{
    public class SearchViewModel : BaseViewModel
    {
        public SearchViewModel()
        {
            SearchCommand = ReactiveCommand.CreateAsyncObservable(
                _country.IsDataAvailableObs.CombineLatest(_session.IsDataAvailableObs, (b, b1) => b && b1),
                _ =>
                {
                    _loading.AddLoadingStatus("Search...");
                   return  _client.SearchAll(SearchTerm, _country.Country.Serialize(), _session.SessionId);
                });

            SearchCommand.Do(_ => _loading.RemoveLoadingStatus("Search...")).BindTo(this, self => self.SearchResult);
            this.WhenAnyValue(self => self.SearchResult).Where(x => x != null).Subscribe(r =>
            {
                SongResult = r.Item1.Select((s, index) => new SongViewModel(s, index + 1)).ToList();
                PlaylistResult = r.Item2.Select(p => new PlaylistViewModel(p, true)).ToList();
                AlbumResult = r.Item4.Select(a => new AlbumViewModel(a)).ToList();
                ArtistResult = r.Item3.Select(a => new ArtistViewModel(a)).ToList();
            });
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
