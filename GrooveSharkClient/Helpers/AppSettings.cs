using System.Linq;
using Windows.Security.Credentials;
using Windows.Storage;

namespace GrooveSharkClient.Helpers
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
            RemoveValue(key);
            _localSettings.Values.Add(key, value);
        }

        public static void RemoveValue(string key)
        {
            if (_localSettings.Values.ContainsKey(key))
                _localSettings.Values.Remove(key);
        }

        public static bool ContainsKey(string key)
        {
            return _localSettings.Values.ContainsKey(key);
        }

        public static void SaveCredential(string userName, string password)
        {
            var vault = new PasswordVault();
            var cred = new PasswordCredential("GrooveSharkCredential", userName, password);
            vault.Add(cred);
        }

        public static PasswordCredential RetrieveCredential()
        {
            var vault = new PasswordVault();
            if (vault.RetrieveAll().Count != 0)
                return vault.FindAllByResource("GrooveSharkCredential").FirstOrDefault();
            return null;
        }


        public static void RemoveCredential()
        {
            var vault = new PasswordVault();
            if (vault.RetrieveAll().Count != 0)
                vault.Remove(vault.FindAllByResource("GrooveSharkCredential").FirstOrDefault());
        }
    }
}
