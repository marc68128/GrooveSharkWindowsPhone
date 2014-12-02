using System;
using System.Diagnostics;
using System.Reactive.Linq;
using GrooveSharkClient.Contracts;
using ReactiveUI;

namespace GrooveSharkClient.Services
{
    public class SessionService : ReactiveObject, ISessionService
    {
        public SessionService(IGrooveSharkClient client, LoadingService loadingService)
        {
            LoadSessionId = ReactiveCommand.CreateAsyncObservable(_ =>
            {
                loadingService.AddLoadingStatus("Stating Session...");
                return client.CreateSession();
            });

            SessionIdObs = LoadSessionId;

            SessionIdObs.Where(s => !string.IsNullOrEmpty(s)).Subscribe(s =>
            {
                Debug.WriteLine("[SessionService] Session : " + s);
                SessionId = s;
                loadingService.RemoveLoadingStatus("Stating Session...");
                IsDataAvailable = true;
            });


            LoadSessionId.Execute(null);
            LoadSessionId.ThrownExceptions.Subscribe(ex =>
            {
                Debug.WriteLine("[SessionService]" + ex);
                ThrownException = ex;
                loadingService.RemoveLoadingStatus("Stating Session...");
                IsDataAvailable = false;
            });
        }

        public IObservable<string> SessionIdObs { get; private set; }
        public string SessionId { get; private set; }

        public ReactiveCommand<string> LoadSessionId { get; private set; }

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
