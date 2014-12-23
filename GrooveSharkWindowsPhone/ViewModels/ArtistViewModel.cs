using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using GrooveSharkClient.Models.Entity;
using GrooveSharkClient.Models.Exception;
using GrooveSharkWindowsPhone.Helpers;
using GrooveSharkWindowsPhone.Views;
using ReactiveUI;

namespace GrooveSharkWindowsPhone.ViewModels
{
    public class ArtistViewModel : BaseViewModel
    {
        public ArtistViewModel()
        {
            
        }
        public ArtistViewModel(Artist artist)
        {
            Id = artist.ArtistID;
            Name = artist.ArtistName;
            Title = Name; 
            SetupCommands(); 
        }

        private void SetupCommands()
        {
            #region LoadArtistAlbumsCommand

            LoadArtistAlbumsCommand = ReactiveCommand.CreateAsyncObservable(_ =>
            {
                _loading.AddLoadingStatus(string.Format("Loading {0} Albums", Name));
                return _client.GetArtistAlbums(_session.SessionId, Id);
            });

            LoadArtistAlbumsCommand.Subscribe(a =>
            {
                _loading.RemoveLoadingStatus(string.Format("Loading {0} Albums", Name));
                Albums = a.Select(al => new AlbumViewModel(al)).ToList();
            });
            LoadArtistAlbumsCommand.ThrownExceptions.OfType<WebException>()
                .Do(_ => _loading.RemoveLoadingStatus(string.Format("Loading {0} Albums", Name)))
                .BindTo(this, self => self.WebException);
            LoadArtistAlbumsCommand.ThrownExceptions.OfType<GrooveSharkException>()
                .Do(_ => _loading.RemoveLoadingStatus(string.Format("Loading {0} Albums", Name)))
                .BindTo(this, self => self.GrooveSharkException);

            #endregion

            #region NavigateToArtistCommand

            NavigateToArtistCommand = ReactiveCommand.Create();
            NavigateToArtistCommand.Subscribe(_ => NavigationHelper.Navigate(typeof (ArtistView), this)); 

            #endregion
        }

        private int _id;
        public int Id
        {
            get { return _id; }
            set { this.RaiseAndSetIfChanged(ref _id, value); }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { this.RaiseAndSetIfChanged(ref _name, value); }
        }


        private List<AlbumViewModel> _albums;
        public List<AlbumViewModel> Albums
        {
            get { return _albums; }
            set { this.RaiseAndSetIfChanged(ref _albums, value); }
        }
        

        public ReactiveCommand<Album[]> LoadArtistAlbumsCommand { get; private set; }
        public ReactiveCommand<object> NavigateToArtistCommand { get; private set; }

    }
}
