using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrooveSharkClient.Contracts;
using GrooveSharkClient.Services;
using Splat;

namespace GrooveSharkWindowsPhone
{
    public static class DI
    {
        public static void Setup()
        {
            var client = new GrooveSharkClient.GrooveSharkClient();
            var sessionService = new SessionService(client); 

            Locator.CurrentMutable.RegisterConstant(client, typeof(IGrooveSharkClient));
            Locator.CurrentMutable.RegisterConstant(sessionService, typeof(ISessionService));
        }
    }
}
