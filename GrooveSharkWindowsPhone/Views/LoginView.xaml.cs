using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
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
using GrooveSharkClient.Contracts;
using GrooveSharkWindowsPhone.ViewModels;
using Microsoft.Practices.Unity;
using ReactiveUI;

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

            ViewModel.ShowDataObs.CombineLatest(ViewModel.ShowLoaderObs, (showData, showLoader) => !showData && !showLoader)
                .Select(b => b ? Visibility.Visible : Visibility.Collapsed)
                .BindTo(UcError, x => x.Visibility);
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
