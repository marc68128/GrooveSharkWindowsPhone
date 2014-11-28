using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556
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
