using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GrooveSharkClient.Models.Entity
{
    public class CountryInfo
    {
        public CountryInfo()
        {
            
        }

        public CountryInfo(GrooveSharkResult r)
        {
            ID = r.Result.ID;
            CC1 = r.Result.CC1;
            CC2 = r.Result.CC2;
            CC3 = r.Result.CC3;
            CC4 = r.Result.CC4;
            DMA = r.Result.DMA;
            IPR = r.Result.IPR;
        }

        [DataMember(Name = "ID")]
        public int ID { get; set; }

        [DataMember(Name = "CC1")]
        public int CC1 { get; set; }

        [DataMember(Name = "CC2")]
        public int CC2 { get; set; }

        [DataMember(Name = "CC3")]
        public int CC3 { get; set; }

        [DataMember(Name = "CC4")]
        public int CC4 { get; set; }

        [DataMember(Name = "DMA")]
        public int DMA { get; set; }

        [DataMember(Name = "IPR")]
        public int IPR { get; set; }

        public string GetCountryInfoAsJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
