using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrooveSharkClient.Contracts;
using GrooveSharkClient.Models.Entity;

namespace GrooveSharkClient.Services
{
    public class CountryService : ICountryService
    {
        public CountryService(IGrooveSharkClient client)
        {
            CountryObs = client.GetCountry(); 
        }

        public IObservable<CountryInfo> CountryObs { get; private set; }
    }
}
