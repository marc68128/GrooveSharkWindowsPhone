using System;
using Windows.UI.Popups;
using Windows.UI.Xaml.Navigation;
using GrooveSharkWindowsPhone.Helpers;
using GrooveSharkWindowsPhone.ViewModels;

namespace GrooveSharkWindowsPhone.Views
{
    public sealed partial class RegisterView : BaseView
    {
        public RegisterView() : base(new RegisterViewModel())
        {
            this.InitializeComponent();
            SetupBindings(); 
        }

        private RegisterViewModel ViewModel { get { return DataContext as RegisterViewModel; } }

        private void SetupBindings()
        {
            ViewModel.GrooveSharkExceptionObs.WhereNotNull().Subscribe(e => new MessageDialog(e.Description).ShowAsync());
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
    }
}
