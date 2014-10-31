﻿using System;
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
        public static void Setup(string username, string password)
        {
            var client = new GrooveSharkClient.GrooveSharkClient();
            var sessionService = new SessionService(client);
            var coutryService = new CountryService(client);
            var userService = new UserService(client, sessionService, username, password);

            Locator.CurrentMutable.RegisterConstant(client, typeof(IGrooveSharkClient));
            Locator.CurrentMutable.RegisterConstant(sessionService, typeof(ISessionService));
            Locator.CurrentMutable.RegisterConstant(coutryService, typeof(ICountryService));
            Locator.CurrentMutable.RegisterConstant(userService, typeof(IUserService));
        }


    }
}
