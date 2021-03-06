﻿using System.Reactive.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556
using GrooveSharkWindowsPhone.ViewModels;
using ReactiveUI;

namespace GrooveSharkWindowsPhone.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsView 
    {
        public SettingsView() : base(new SettingsViewModel())
        {
            this.InitializeComponent();
            ViewModel.RefreshCurrentUserCommand.Execute(null);

            ViewModel.IsUserConnectedObs.Select(u => u ? Visibility.Collapsed : Visibility.Visible)
                .BindTo(NoLoginGrid, grid => grid.Visibility);
            ViewModel.IsUserConnectedObs.Select(u => u ? Visibility.Visible : Visibility.Collapsed)
                .BindTo(LoginGrid, grid => grid.Visibility);
        }

        private SettingsViewModel ViewModel
        {
            get { return DataContext as SettingsViewModel; }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }


    }
}
