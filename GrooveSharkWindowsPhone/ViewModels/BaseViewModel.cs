using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Controls;
using GrooveSharkClient.Contracts;
using GrooveSharkClient.Models;
using ReactiveUI;
using Splat;

namespace GrooveSharkWindowsPhone.ViewModels
{
    public class BaseViewModel : ReactiveObject
    {
        protected IGrooveSharkClient _client;
        protected ISessionService _session;
        protected ICountryService _country;
        protected IUserService _user; 

        public BaseViewModel()
        {
            _client = Locator.Current.GetService<IGrooveSharkClient>();
            _session = Locator.Current.GetService<ISessionService>();
            _country = Locator.Current.GetService<ICountryService>();
            _user = Locator.Current.GetService<IUserService>();

            _session.IsLoadingObs
                .CombineLatest(_user.IsLoadingObs, (b, b1) => b || b1)
                .Do(b => Debug.WriteLine("[BaseViewModel] ShowLoader : " + b))
                .ObserveOn(RxApp.MainThreadScheduler)
                .BindTo(this, self => self.ShowLoader);

            _session.IsDataAvailableObs
                .CombineLatest(_user.IsUserAvailableObs, (b, b1) => b && b1)
                .Do(b => Debug.WriteLine("[BaseViewModel] ShowData : " + b))
                .ObserveOn(RxApp.MainThreadScheduler)
                .BindTo(this, self => self.ShowData);
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { this.RaiseAndSetIfChanged(ref _title, value); }
        }

        private bool _showLoader;
        public bool ShowLoader
        {
            get { return _showLoader; }
            set { this.RaiseAndSetIfChanged(ref _showLoader, value); }
        }
        public IObservable<bool> ShowLoaderObs { get { return this.WhenAnyValue(self => self.ShowLoader); } } 

        private bool _showData;
        public bool ShowData
        {
            get { return _showData; }
            set { this.RaiseAndSetIfChanged(ref _showData, value); }
        }
        public IObservable<bool> ShowDataObs { get { return this.WhenAnyValue(self => self.ShowData); } } 

        private string _status;
        public string Status
        {
            get { return _status; }
            set { this.RaiseAndSetIfChanged(ref _status, value); }
        }

        public string AppTitle { get { return "GrooveShark"; } }

    }
}
