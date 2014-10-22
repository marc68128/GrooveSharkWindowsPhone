using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace GrooveSharkWindowsPhone.Helpers
{
    public static class AppSettings
    {
        private static ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings; 
        public static object GetValue(string key)
        {
            if (_localSettings.Values.ContainsKey(key))
                return _localSettings.Values[key]; 
            return null;
        }

        public static void AddValue(string key, object value)
        {
            _localSettings.Values.Add(key, value);
        }

        public static IObservable<string> SessionIdObs { get; set; } 
    }
}
