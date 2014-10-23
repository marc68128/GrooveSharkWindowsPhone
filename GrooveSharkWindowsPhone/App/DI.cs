using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrooveSharkClient.Contracts;
using GrooveSharkClient.Services;
using GrooveSharkWindowsPhone.Helpers;
using Splat;

namespace GrooveSharkWindowsPhone
{
    public static class DI
    {
        public static void Setup()
        {
            var client = new GrooveSharkClient.GrooveSharkClient();
            var sessionService = new SessionService(client, AppSettings.GetValue("UserName") as string, AppSettings.GetValue("Md5Password") as string);
            var coutryService = new CountryService(client);

            Locator.CurrentMutable.RegisterConstant(client, typeof(IGrooveSharkClient));
            Locator.CurrentMutable.RegisterConstant(sessionService, typeof(ISessionService));
            Locator.CurrentMutable.RegisterConstant(coutryService, typeof(ICountryService));
        }
    }
}
