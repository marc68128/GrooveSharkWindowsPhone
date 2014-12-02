using System;
using GrooveSharkClient.Models.Entity;
using ReactiveUI;

namespace GrooveSharkClient.Contracts
{
    public interface ICountryService : IService
    {
        IObservable<CountryInfo> CountryObs { get; }
        CountryInfo Country { get; }

        ReactiveCommand<CountryInfo> LoadCountryCommand { get; }
    }
}
