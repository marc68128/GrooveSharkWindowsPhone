﻿using System;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GrooveSharkWindowsPhone.Helpers;
using GrooveSharkWindowsPhone.ViewModels;

namespace GrooveSharkWindowsPhone.Views
{
    public class BaseView : Page
    {
        public BaseView(BaseViewModel viewModel)
        {
            DataContext = viewModel;

            var statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
            statusBar.BackgroundColor = Windows.UI.Color.FromArgb(255, 248, 111, 5);
            statusBar.BackgroundOpacity = 1;
            statusBar.ShowAsync();

            (DataContext as BaseViewModel).StatusObs.WhereNotNull().Subscribe(s => {
                statusBar.ProgressIndicator.Text = s;
            });
            (DataContext as BaseViewModel).ShowLoaderObs.Subscribe(x => {
                if (x)
                    statusBar.ProgressIndicator.ShowAsync();
                else
                    statusBar.ProgressIndicator.HideAsync();
            });

            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            Frame frame = Window.Current.Content as Frame;
            if (frame == null)
            {
                return;
            }

            if (frame.CanGoBack)
            {
                frame.GoBack();
                e.Handled = true;
            }
        }
    }
}
