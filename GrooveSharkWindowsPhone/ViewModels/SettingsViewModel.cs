using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace GrooveSharkWindowsPhone.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        public SettingsViewModel()
        {
            Title = "Settings";
            if (!string.IsNullOrEmpty(_session.SessionId))
            {
                var userInfo = _client.GetUserInfo(_session.SessionId);
                userInfo.Subscribe(u =>
              {

              });
            }


        }
    }
}
