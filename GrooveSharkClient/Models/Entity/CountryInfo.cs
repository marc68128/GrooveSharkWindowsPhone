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

        public int ID { get; set; }
        public int CC1 { get; set; }
        public int CC2 { get; set; }
        public int CC3 { get; set; }
        public int CC4 { get; set; }
        public int DMA { get; set; }
        public int IPR { get; set; }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
