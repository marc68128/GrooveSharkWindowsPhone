using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Controls;
using GrooveSharkClient.Contracts;
using ReactiveUI;
using Splat;

namespace GrooveSharkWindowsPhone.ViewModels
{
    public class BaseViewModel : ReactiveObject
    {
        protected IGrooveSharkClient _client;
        protected ISessionService _session;
        protected ICountryService _country; 

        public BaseViewModel()
        {
            _client = Locator.Current.GetService<IGrooveSharkClient>();
            _session = Locator.Current.GetService<ISessionService>();
            _country = Locator.Current.GetService<ICountryService>();
            IsLoading = true; 

        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { this.RaiseAndSetIfChanged(ref _title, value); }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { this.RaiseAndSetIfChanged(ref _isLoading, value); }
        }

        private string _status;
        public string Status
        {
            get { return _status; }
            set { this.RaiseAndSetIfChanged(ref _status, value); }
        }

        public string AppTitle { get { return "GrooveShark"; } }

    }
}
