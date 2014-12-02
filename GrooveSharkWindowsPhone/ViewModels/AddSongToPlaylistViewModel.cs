using System;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using Windows.UI.Popups;
using GrooveSharkClient.Models;
using GrooveSharkClient.Models.Exception;
using GrooveSharkWindowsPhone.Helpers;
using ReactiveUI;

namespace GrooveSharkWindowsPhone.ViewModels
{
    public class AddSongToPlaylistViewModel : BaseViewModel
    {
        public AddSongToPlaylistViewModel()
        {
            Title = "Choose a playlist";
            PlaylistsViewModel = new PlaylistsViewModel();

            SetupCommands();

            this.WhenAnyValue(self => self.SelectedPlaylist)
                .WhereNotNull()
                .Subscribe(_ =>
                {
                    SelectedPlaylist.LoadPlaylistSongCommand.Execute(null);
                    SelectedPlaylist.LoadPlaylistSongCommand.Subscribe(s =>
                    {
                        var newPlaylist = s.Songs.Select(sn => sn.SongID).ToList();
                        newPlaylist.AddRange(SongIds);
                        SongIds = newPlaylist.ToArray();
                        AddSongToSelectedPlaylistCommand.Execute(null);
                    });
                    
                });
        }

        private void SetupCommands()
        {
            ToggleAddFormCommand = ReactiveCommand.Create();
            ToggleAddFormCommand.Subscribe(_ =>
            {
                IsAddFormOpen = !IsAddFormOpen;
            });

            #region AddPlaylistCommand

            var canAddPlaylist =
                this.WhenAnyValue(self => self.PlaylistName)
                    .Select(p => !string.IsNullOrEmpty(p) && p.Count() > 1)
                    .CombineLatest(this.WhenAnyValue(self => self.SongIds).Select(s => s != null && s.Any()),
                        (a, b) => a && b);

            AddPlaylistCommand = ReactiveCommand.CreateAsyncObservable(canAddPlaylist, _ =>
            {
                _loading.AddLoadingStatus("Adding playlist...");
                return _client.AddPlaylist(SongIds, PlaylistName, _session.SessionId);
            });

            AddPlaylistCommand.Subscribe(b =>
            {
                _loading.RemoveLoadingStatus("Adding playlist...");
                new MessageDialog("OK").ShowAsync();
                NavigationHelper.GoBack();
            });

            AddPlaylistCommand.ThrownExceptions.OfType<WebException>()
                .Do(_ => _loading.RemoveLoadingStatus("Adding playlist..."))
                .BindTo(this, self => self.WebException);
            AddPlaylistCommand.ThrownExceptions.OfType<GrooveSharkException>()
                .Do(_ => _loading.RemoveLoadingStatus("Adding playlist..."))
                .BindTo(this, self => self.GrooveSharkException);

            #endregion

            #region AddSongToSelectedPlaylistCommand

            AddSongToSelectedPlaylistCommand = ReactiveCommand.CreateAsyncObservable(this.WhenAnyValue(self => self.SongIds).Select(s => s != null && s.Any()), _ =>
            {
                _loading.AddLoadingStatus("Adding song to playlist...");
                return _client.SetPlaylistSongs(SongIds, SelectedPlaylist.PlaylistId, _session.SessionId);
            });

            AddSongToSelectedPlaylistCommand.Subscribe(b =>
            {
                _loading.RemoveLoadingStatus("Adding song to playlist...");
                new MessageDialog("OK").ShowAsync();
                NavigationHelper.GoBack();
            });

            AddSongToSelectedPlaylistCommand.ThrownExceptions.OfType<WebException>()
                .Do(_ => _loading.RemoveLoadingStatus("Adding song to playlist..."))
                .BindTo(this, self => self.WebException);
            AddSongToSelectedPlaylistCommand.ThrownExceptions.OfType<GrooveSharkException>()
                .Do(_ => _loading.RemoveLoadingStatus("Adding song to playlist..."))
                .BindTo(this, self => self.GrooveSharkException);

            #endregion

        }

        private PlaylistsViewModel _playlistsViewModel;
        public PlaylistsViewModel PlaylistsViewModel
        {
            get { return _playlistsViewModel; }
            private set { this.RaiseAndSetIfChanged(ref _playlistsViewModel, value); }
        }

        private bool _isAddFormOpen;
        public bool IsAddFormOpen
        {
            get { return _isAddFormOpen; }
            private set { this.RaiseAndSetIfChanged(ref _isAddFormOpen, value); }
        }

        private string _playlistName;
        public string PlaylistName
        {
            get { return _playlistName; }
            set { this.RaiseAndSetIfChanged(ref _playlistName, value); }
        }

        private int[] _songIds;
        public int[] SongIds
        {
            get { return _songIds; }
            set { this.RaiseAndSetIfChanged(ref _songIds, value); }
        }

        private PlaylistViewModel _selectedPlaylist;
        public PlaylistViewModel SelectedPlaylist
        {
            get { return _selectedPlaylist; }
            set { this.RaiseAndSetIfChanged(ref _selectedPlaylist, value); }
        }



        public ReactiveCommand<object> ToggleAddFormCommand { get; private set; }
        public ReactiveCommand<bool> AddPlaylistCommand { get; private set; }
        private ReactiveCommand<bool> AddSongToSelectedPlaylistCommand { get; set; }
    }
}
