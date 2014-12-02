using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556
using GrooveSharkWindowsPhone.ViewModels;

namespace GrooveSharkWindowsPhone.Views
{
    public sealed partial class PlayerView 
    {
        public PlayerView() : base(new PlayerViewModel())
        {
            this.InitializeComponent();

        }

        private PlayerViewModel ViewModel
        {
            get { return DataContext as PlayerViewModel; }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
    }
}
