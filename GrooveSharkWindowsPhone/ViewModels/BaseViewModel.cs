using GrooveSharkClient.Contracts;
using GrooveSharkClient.Models;
using GrooveSharkClient.Models.Exception;
using GrooveSharkWindowsPhone.AudioPlayer;
using ReactiveUI;
using Splat;
using System;
using System.Net;
using System.Reactive.Linq;

namespace GrooveSharkWindowsPhone.ViewModels
{
    public class BaseViewModel : ReactiveObject
    {
        protected IGrooveSharkClient _client;
        protected ISessionService _session;
        protected ICountryService _country;
        protected IUserService _user;
        protected ILoadingService _loading;
        protected IAudioPlayerService _audioPlayer;

        public BaseViewModel()
        {
            _client = Locator.Current.GetService<IGrooveSharkClient>();
            _session = Locator.Current.GetService<ISessionService>();
            _country = Locator.Current.GetService<ICountryService>();
            _user = Locator.Current.GetService<IUserService>();
            _loading = Locator.Current.GetService<ILoadingService>();
            _audioPlayer = Locator.Current.GetService<IAudioPlayerService>();


            ShowSessionErrorObs = _loading.IsLoadingObs.CombineLatest(_session.IsDataAvailableObs, (b, b1) => !b && !b1);


            _session.ThrownExceptionObs.OfType<WebException>().BindTo(this, self => self.WebException);
            _session.ThrownExceptionObs.OfType<GrooveSharkException>().BindTo(this, self => self.GrooveSharkException);

            _user.ThrownExceptionObs.OfType<WebException>().BindTo(this, self => self.WebException);
            _user.ThrownExceptionObs.OfType<GrooveSharkException>().BindTo(this, self => self.GrooveSharkException);
        }


        private string _title;
        public string Title
        {
            get { return _title; }
            set { this.RaiseAndSetIfChanged(ref _title, value); }
        }

        public IObservable<bool> ShowLoaderObs
        {
            get
            {
                return this._loading.IsLoadingObs;
            }
        }
        public IObservable<bool> ShowSessionErrorObs { get; private set; }
        public IObservable<string> StatusObs
        {
            get
            {
                return this._loading.CurrentStatusObs;
            }
        }
      

        public string AppTitle { get { return "GrooveShark"; } }

        #region Exception

        private WebException _webException;
        protected WebException WebException
        {
            get { return _webException; }
            set { this.RaiseAndSetIfChanged(ref _webException, value); }
        }
        public IObservable<WebException> WebExceptionObs { get { return this.WhenAnyValue(self => self.WebException); } }

        private GrooveSharkException _grooveSharkException;
        protected GrooveSharkException GrooveSharkException
        {
            get { return _grooveSharkException; }
            set { this.RaiseAndSetIfChanged(ref _grooveSharkException, value); }
        }
        public IObservable<GrooveSharkException> GrooveSharkExceptionObs { get { return this.WhenAnyValue(self => self.GrooveSharkException); } }

        #endregion

    }
}
