using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.Web.Syndication;
using GrooveSharkClient.Contracts;
using GrooveSharkClient.Models;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using ReactiveUI;

namespace GrooveSharkClient.Services
{
    public class SessionService : ReactiveObject, ISessionService
    {
        public SessionService(IGrooveSharkClient client)
        {

            LoadSessionId = ReactiveCommand.CreateAsyncObservable(_ =>
            {
                IsLoading = true; 
                return client.CreateSession();
            });

            SessionIdObs = LoadSessionId;

            SessionIdObs.Where(s => !string.IsNullOrEmpty(s)).Subscribe(s =>
            { 
                Debug.WriteLine("[SessionService] Session : " + s);
                SessionId = s; 
                IsLoading = false;
                IsSessionAvailable = true;
            });


            LoadSessionId.Execute(null);
            LoadSessionId.ThrownExceptions.Subscribe(ex =>
            {
                Debug.WriteLine("[SessionService]" + ex);
                IsLoading = false;
                IsSessionAvailable = false; 
            });


        }

        public IObservable<string> SessionIdObs { get; private set; }
        public string SessionId { get; private set; }

        public ReactiveCommand<string> LoadSessionId { get; private set; }

        #region Loading

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            private set { this.RaiseAndSetIfChanged(ref _isLoading, value); }
        }
        public IObservable<bool> IsLoadingObs
        {
            get { return this.WhenAnyValue(self => self.IsLoading); }
        }

        private bool _isSessionAvailable;
        public bool IsSessionAvailable
        {
            get { return _isSessionAvailable; }
            set { this.RaiseAndSetIfChanged(ref _isSessionAvailable, value); }
        }
        public IObservable<bool> IsSessionAvailableObs
        {
            get { return this.WhenAnyValue(self => self.IsSessionAvailable); }
        }

        #endregion

    }
}
