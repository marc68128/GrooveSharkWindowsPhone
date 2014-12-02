using System;
using System.Reactive.Linq;
using Windows.UI.Popups;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556
using GrooveSharkWindowsPhone.ViewModels;

namespace GrooveSharkWindowsPhone.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginView 
    {
        public LoginView() : base(new LoginViewModel())
        {
            this.InitializeComponent();

            ViewModel.WebExceptionObs.Where(ex => ex != null).Subscribe(ex => new MessageDialog("No network !").ShowAsync());
            ViewModel.GrooveSharkExceptionObs.Where(ex => ex != null).Subscribe(ex => new MessageDialog(ex.Description).ShowAsync());

        }

        private LoginViewModel ViewModel
        {
            get { return DataContext as LoginViewModel; }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

        }
    }
}
