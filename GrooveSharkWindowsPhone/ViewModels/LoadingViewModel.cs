using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using GrooveSharkClient.Helpers;
using GrooveSharkClient.Models.Exception;
using ReactiveUI;

namespace GrooveSharkWindowsPhone.ViewModels
{
    public abstract class LoadingViewModel <T> : BaseViewModel
    {
        protected LoadingViewModel(string loadingStatus)
        {
            Data = new ReactiveList<T>();

            LoadDataCommand = ReactiveCommand.CreateAsyncObservable(CanLoadData, _ =>
            {
                    _loading.AddLoadingStatus(loadingStatus);
                    return LoadData();
            });

            LoadDataCommand.Where(p => p != null).Subscribe(p =>
            {
                _loading.RemoveLoadingStatus(loadingStatus);
                Data.Clear();
                Data.AddRange(p);
            });

            LoadDataCommand.ThrownExceptions.OfType<WebException>().Do(_ => _loading.RemoveLoadingStatus(loadingStatus)).BindTo(this, self => self.WebException);
            LoadDataCommand.ThrownExceptions.OfType<GrooveSharkException>().Do(_ => _loading.RemoveLoadingStatus(loadingStatus)).BindTo(this, self => self.GrooveSharkException);

            LoadDataCommand.ThrownExceptions.Subscribe(e =>
            {
                Logger.Log(new Log
                {
                    Title = "Loading Error : " + loadingStatus,
                    Description = e.Message,
                    Level = 3, 
                    StackTrace = e.StackTrace
                });
            });

#if DEBUG
            LoadDataCommand.ThrownExceptions.Subscribe(e => (new MessageDialog(e.Message)).ShowAsync());
#endif

        }

        protected ReactiveList<T> Data { get; set; }

        public ReactiveCommand<List<T>> LoadDataCommand { get; private set; }

        protected abstract IObservable<List<T>> LoadData();

        protected abstract IObservable<bool> CanLoadData { get; }
    }
}
