using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641
using GrooveSharkClient.Models;
using GrooveSharkWindowsPhone.Helpers;
using ReactiveUI;
using xBrainLab.Security.Cryptography;
using ReactiveCommand = ReactiveUI.Legacy.ReactiveCommand;

namespace GrooveSharkWindowsPhone
{

    public sealed partial class MainPage : Page
    {
        public string SessionID { get; set; }
        public User User { get; set; }

        public ReactiveCommand ConnectUserCommand { get; set; }

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
          
            AppSettings.SessionIdObs.BindTo(this, self => self.SessionID);
            var client = new GrooveSharkClient.GrooveSharkClient();
            var sub = new Subject<bool>();
            if (SessionID == null)
             sub.OnNext(false);

            ConnectUserCommand = new ReactiveCommand(sub);
            var user = ConnectUserCommand.RegisterAsync(_ => client.Login("marc68128", MD5.GetHashString("themarc68"), SessionID));

            user.ObserveOn(RxApp.MainThreadScheduler).Subscribe(u =>
            {
                MessageDialog messageDialog = new MessageDialog("Vous êtes bien connecté (" + u.FName + ")");
                messageDialog.ShowAsync();
            });

            AppSettings.SessionIdObs.Subscribe(s => sub.OnNext(!string.IsNullOrEmpty(s)));


            Button.Command = ConnectUserCommand;
        }
    }
}
