using GrooveSharkClient.Contracts;
using GrooveSharkClient.Services;
using GrooveSharkWindowsPhone.AudioPlayer;
using Splat;

namespace GrooveSharkWindowsPhone
{
    public static class DI
    {
        public static void Setup(string username, string password)
        {
            var client = new GrooveSharkClient.GrooveSharkClient();
            var audioPlayerService = new AudioPlayerService(); 
            var loadingService = new LoadingService();
            var sessionService = new SessionService(client, loadingService);
            var coutryService = new CountryService(client, loadingService);
            var userService = new UserService(client, sessionService, loadingService, username, password);
           

            Locator.CurrentMutable.RegisterConstant(client, typeof(IGrooveSharkClient));
            Locator.CurrentMutable.RegisterConstant(audioPlayerService, typeof(IAudioPlayerService));
            Locator.CurrentMutable.RegisterConstant(loadingService, typeof(ILoadingService));
            Locator.CurrentMutable.RegisterConstant(sessionService, typeof(ISessionService));
            Locator.CurrentMutable.RegisterConstant(coutryService, typeof(ICountryService));
            Locator.CurrentMutable.RegisterConstant(userService, typeof(IUserService));
        }


    }
}
