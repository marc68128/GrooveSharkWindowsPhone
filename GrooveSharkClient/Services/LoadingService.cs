using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using GrooveSharkClient.Contracts;
using ReactiveUI;

namespace GrooveSharkClient.Services
{
    public class LoadingService : ReactiveObject, ILoadingService
    {
        public LoadingService()
        {
            LoadingStatus = new List<string>();
        }

        public async void AddLoadingStatus(string status)
        {
            LoadingStatus.Add(status);

            TaskFactory factory = new TaskFactory();
            factory.StartNew(() =>
            {
                if (!IsLoading)
                {
                    Debug.WriteLine("[LoadingService] Loading Start");

                    CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        IsLoading = true;
                        CurrentStatus = status;
                    });
                }
            });
        }

        public void RemoveLoadingStatus(string status)
        {
            if (!LoadingStatus.Contains(status))
            {
                Debug.WriteLine("[Warning] You tried to remove a loading status who doesn't exist");
                return;
            }
            LoadingStatus.Remove(status);
             TaskFactory factory = new TaskFactory();
            factory.StartNew(() =>
            {
                CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    if (CurrentStatus == status && LoadingStatus.Any())
                    {
                        CurrentStatus = LoadingStatus.First();
                    }
                    else if (CurrentStatus == status && LoadingStatus.Count == 0)
                    {
                        Debug.WriteLine("[LoadingService] Loading End");

                        CurrentStatus = null;
                        IsLoading = false;
                    }
                });
            });
        }

        protected List<string> LoadingStatus { get; set; }


        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            private set { this.RaiseAndSetIfChanged(ref _isLoading, value); }
        }
        public IObservable<bool> IsLoadingObs { get { return this.WhenAnyValue(self => self.IsLoading); } }

        private string _currentStatus;
        public string CurrentStatus
        {
            get { return _currentStatus; }
            private set { this.RaiseAndSetIfChanged(ref _currentStatus, value); }
        }
        public IObservable<string> CurrentStatusObs { get { return this.WhenAnyValue(self => self.CurrentStatus); } }

    }
}
