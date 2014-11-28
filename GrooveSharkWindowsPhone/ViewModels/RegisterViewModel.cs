using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Reactive.Windows.Foundation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Popups;
using GrooveSharkClient.Models;
using GrooveSharkWindowsPhone.Helpers;
using ReactiveUI;
using xBrainLab.Security.Cryptography;

namespace GrooveSharkWindowsPhone.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        public RegisterViewModel()
        {

            Title = "Register";
            UserNameLabel = "UserName";
            PasswordLabel = "Password";
            FirstNameLabel = "First Name";
            LastNameLabel = "Last Name";
            EmailLabel = "Email Address";

            if (_user.ConnectedUser != null && _user.ConnectedUser.UserID != 0)
            {
                var res = new MessageDialog("You are already connected !\nIf you want to create a new account please logout first.").ShowAsync();
                res.ToObservable().Subscribe(_ => NavigationHelper.GoBack());
            }

            SetupFormularValidation();
            InitCommands();

            _user.ConnectedUserObs.Where(u => u != null && u.UserID != 0).Take(1).Subscribe(u => {
                AppSettings.SaveCredential(UserName, MD5.GetHashString(Password));
                var res = new MessageDialog("Register success !\nYou are connected as " + u.FName).ShowAsync();
                res.ToObservable().Subscribe(_ => NavigationHelper.GoBack());
            });

        }

        private void SetupFormularValidation()
        {
            var mailRegex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

            IsUserNameValidObs =
                this.WhenAnyValue(self => self.UserName)
                    .Select(u => string.IsNullOrEmpty(u) || (u.Count() > 5 && u.Count() < 32));

            IsPasswordValidObs =
                 this.WhenAnyValue(self => self.Password)
                    .Select(u => !string.IsNullOrEmpty(u) && (u.Count() > 5 && u.Count() < 32));

            IsLastNameValidObs =
                this.WhenAnyValue(self => self.LastName)
                    .Select(u => !string.IsNullOrEmpty(u));

            IsFirstNameValidObs =
                this.WhenAnyValue(self => self.FirstName)
                    .Select(u => !string.IsNullOrEmpty(u));

            IsEmailValidObs =
                this.WhenAnyValue(self => self.Email)
                    .Select(u => !string.IsNullOrEmpty(u) && mailRegex.IsMatch(u));

            IsFormValidObs =
                IsEmailValidObs
                    .CombineLatest(IsPasswordValidObs, (b1, b2) => b1 && b2)
                    .CombineLatest(IsUserNameValidObs, (b1, b2) => b1 && b2)
                    .CombineLatest(IsLastNameValidObs, (b1, b2) => b1 && b2)
                    .CombineLatest(IsFirstNameValidObs, (b1, b2) => b1 && b2);
        }

        private void InitCommands()
        {
            RegisterCommand = ReactiveCommand.CreateAsyncObservable(IsFormValidObs, _ => {
                _loading.AddLoadingStatus("Register...");
                return _client.Register(Email, Password, FullName, _session.SessionId, UserName);
            });

            RegisterCommand.Subscribe(u => {
                _loading.RemoveLoadingStatus("Register...");
                _user.RefreshConnectedUserCommand.Execute(null);
            });

            RegisterCommand.ThrownExceptions.OfType<WebException>().Do(x => _loading.RemoveLoadingStatus("Register...")).BindTo(this, self => self.WebException);
            RegisterCommand.ThrownExceptions.OfType<GrooveSharkException>().Do(x => _loading.RemoveLoadingStatus("Register...")).BindTo(this, self => self.GrooveSharkException);
        }

        #region UserInputs

        private string _email;
        public string Email
        {
            get { return _email; }
            set { this.RaiseAndSetIfChanged(ref _email, value); }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { this.RaiseAndSetIfChanged(ref _password, value); }
        }

        private string _firstName;
        public string FirstName
        {
            get { return _firstName; }
            set { this.RaiseAndSetIfChanged(ref _firstName, value); }
        }

        private string _lastName;
        public string LastName
        {
            get { return _lastName; }
            set { this.RaiseAndSetIfChanged(ref _lastName, value); }
        }

        private string FullName { get { return FirstName + " " + LastName; } }

        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set { this.RaiseAndSetIfChanged(ref _userName, value); }
        }

        #endregion

        #region Labels

        private string _userNameLabel;
        public string UserNameLabel
        {
            get { return _userNameLabel; }
            set { this.RaiseAndSetIfChanged(ref _userNameLabel, value); }
        }

        private string _passwordLabel;
        public string PasswordLabel
        {
            get { return _passwordLabel; }
            set { this.RaiseAndSetIfChanged(ref _passwordLabel, value); }
        }

        private string _emailLabel;
        public string EmailLabel
        {
            get { return _emailLabel; }
            set { this.RaiseAndSetIfChanged(ref _emailLabel, value); }
        }

        private string _firstNameLabel;
        public string FirstNameLabel
        {
            get { return _firstNameLabel; }
            set { this.RaiseAndSetIfChanged(ref _firstNameLabel, value); }
        }

        private string _lastNameLabel;
        public string LastNameLabel
        {
            get { return _lastNameLabel; }
            set { this.RaiseAndSetIfChanged(ref _lastNameLabel, value); }
        }

        #endregion

        #region Validators

        public IObservable<bool> IsUserNameValidObs { get; set; }
        public IObservable<bool> IsPasswordValidObs { get; set; }
        public IObservable<bool> IsLastNameValidObs { get; set; }
        public IObservable<bool> IsFirstNameValidObs { get; set; }
        public IObservable<bool> IsEmailValidObs { get; set; }
        private IObservable<bool> IsFormValidObs { get; set; }

        #endregion

        public ReactiveCommand<User> RegisterCommand { get; set; }

    }
}
