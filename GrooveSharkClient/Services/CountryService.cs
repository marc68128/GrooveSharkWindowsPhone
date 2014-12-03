using System;
using System.Diagnostics;
using System.Reactive.Linq;
using Windows.Foundation.Collections;
using Windows.Media.Playback;
using GrooveSharkClient.Contracts;
using GrooveSharkClient.Models.Entity;
using GrooveSharkShared;
using ReactiveUI;

namespace GrooveSharkClient.Services
{
    public class CountryService : ReactiveObject, ICountryService
    {
        public CountryService(IGrooveSharkClient client, LoadingService loadingService)
        {
            LoadCountryCommand = ReactiveCommand.CreateAsyncObservable(_ => {
                loadingService.AddLoadingStatus("Loading Country...");
                return client.GetCountry();
            });

            LoadCountryCommand.Where(c => c != null).Subscribe(c => {
                loadingService.RemoveLoadingStatus("Loading Country...");
                IsDataAvailable = true;
                Country = c;
                var valueSet = new ValueSet();
                valueSet.Add(Constants.CountryInfosChanged, c.Serialize());
                BackgroundMediaPlayer.SendMessageToBackground(valueSet);
            });

            LoadCountryCommand.ThrownExceptions.Subscribe(ex => {
                Debug.WriteLine("[CountryService]" + ex);
                ThrownException = ex;
                loadingService.RemoveLoadingStatus("Loading Country...");
                IsDataAvailable = false;
            });

            LoadCountryCommand.Execute(null);
        }

        private CountryInfo _country;
        public CountryInfo Country
        {
            get { return _country; }
            private set { this.RaiseAndSetIfChanged(ref _country, value); }
        }
        public IObservable<CountryInfo> CountryObs
        {
            get { return this.WhenAnyValue(self => self.Country); }
        }

        public ReactiveCommand<CountryInfo> LoadCountryCommand { get; private set; }

        #region Loading

        private bool _isDataAvailable;
        public bool IsDataAvailable
        {
            get { return _isDataAvailable; }
            private set { this.RaiseAndSetIfChanged(ref _isDataAvailable, value); }
        }
        public IObservable<bool> IsDataAvailableObs
        {
            get { return this.WhenAnyValue(self => self.IsDataAvailable); }
        }

        #endregion

        #region Exceptions

        private Exception _thrownException;
        public Exception ThrownException
        {
            get { return _thrownException; }
            private set { this.RaiseAndSetIfChanged(ref _thrownException, value); }
        }
        public IObservable<Exception> ThrownExceptionObs
        {
            get
            {
                return this.WhenAnyValue(self => self.ThrownException);
            }
        }

        #endregion
    }
}
