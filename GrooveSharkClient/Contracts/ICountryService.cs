using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrooveSharkClient.Models.Entity;

namespace GrooveSharkClient.Contracts
{
    public interface ICountryService
    {
        IObservable<CountryInfo> CountryObs { get; }
    }
}
