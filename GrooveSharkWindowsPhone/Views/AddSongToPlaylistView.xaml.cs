using System;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556
using GrooveSharkWindowsPhone.ViewModels;
using ReactiveUI;

namespace GrooveSharkWindowsPhone.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddSongToPlaylistView
    {
        public AddSongToPlaylistView()
            : base(new AddSongToPlaylistViewModel())
        {
            this.InitializeComponent();
            ViewModel.PlaylistsViewModel.LoadUserPlaylistsCommand.Execute(null);

            SetupBindings();
        }

        private void SetupBindings()
        {
            ViewModel.PlaylistsViewModel.UserPlaylists.Changed.Subscribe(
                _ => PlaylistList.ItemsSource = ViewModel.PlaylistsViewModel.UserPlaylists);

            ViewModel.WhenAnyValue(vm => vm.IsAddFormOpen).Subscribe(b => {
                if (b)
                    OpenAddForm.Begin();
                else
                    CloseAddForm.Begin();
            });
        }

        private AddSongToPlaylistViewModel ViewModel { get { return DataContext as AddSongToPlaylistViewModel; } }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.SongIds = e.Parameter as int[];
        }

    }
}
