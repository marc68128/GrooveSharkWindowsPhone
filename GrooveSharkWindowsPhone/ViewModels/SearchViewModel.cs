using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using GrooveSharkClient.Models;
using GrooveSharkClient.Models.Entity;
using ReactiveUI;

namespace GrooveSharkWindowsPhone.ViewModels
{
    public class SearchViewModel : BaseViewModel
    {
        public SearchViewModel()
        {
            SearchCommand = ReactiveCommand.CreateAsyncObservable(_ => _country.CountryObs.SelectMany(c => _session.SessionIdObs.SelectMany(s => _client.SearchAll("World on fire", c.GetCountryInfoAsJsonString(), s))));
            SearchCommand.BindTo(this, self => self.SearchResult);
            this.WhenAnyValue(self => self.SearchResult).Where(x => x != null).Subscribe(r =>
            {
                SongResult = r.Item1.Select((s, index) => new SongViewModel(s, index + 1));
            });
            ShowLoader = false; 
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

        private IEnumerable<SongViewModel> _songResult;

        public IEnumerable<SongViewModel> SongResult
        {
            get { return _songResult; }
            set { this.RaiseAndSetIfChanged(ref _songResult, value); }
        }



        public ReactiveCommand<Tuple<Song[], Playlist[], Artist[], Album[]>> SearchCommand { get; set; }
    }
}
