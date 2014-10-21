using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using GrooveSharkClient.Contracts;
using GrooveSharkClient.Models;
using GrooveSharkWindowsPhone.Helpers;
using GrooveSharkWindowsPhone.Views;
using ReactiveUI;
using Splat;

namespace GrooveSharkWindowsPhone.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {

        public HomeViewModel() 
        {
            PopularSongsToday = new ReactiveList<SongViewModel>();

            LoadPopularSongsTodayCommand = ReactiveCommand.CreateAsyncObservable(o => _session.SessionIdObs.SelectMany(session => _client.GetPopularSongToday(session)));
            LoadPopularSongsTodayCommand.Subscribe(s =>
            {
                PopularSongsToday.Clear();
                PopularSongsToday.AddRange(s.Select((x, index) => new SongViewModel(x, index + 1)));
                IsLoading = false;
                Status = "";
            });
            LoadPopularSongsTodayCommand.ThrownExceptions.Do(e => IsLoading = false).Subscribe(e => Debug.WriteLine(e.Message));

            LoadPopularSongsTodayCommand.Execute(null);
            IsLoading = true;
            Status = "Loading";

            //var popularSongsObs = _session.SessionIdObs.SelectMany(session => _client.GetPopularSongToday(session));


            //popularSongsObs.ObserveOn(RxApp.MainThreadScheduler).Subscribe(s =>
            //{
            //    PopularSongsToday.Clear();
            //    PopularSongsToday.AddRange(s.Select((x, index) => new SongViewModel(x, index + 1)));
            //    IsLoading = false;
            //    Status = "";
            //});



            NavigateToSettingsCommand = ReactiveCommand.Create();
            NavigateToSettingsCommand.Subscribe(_ => NavigationHelper.Navigate(typeof (SettingsView)));
        }

        public ReactiveList<SongViewModel> PopularSongsToday { get; set; }

        public ReactiveCommand<Song[]> LoadPopularSongsTodayCommand { get; set; }
        public ReactiveCommand<object> NavigateToSettingsCommand { get; set; }
    }
}
