using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GrooveSharkClient.Models.Entity
{
    public class StreamInfo
    {
        public StreamInfo()
        {
            
        }
        public StreamInfo(GrooveSharkResult r)
        {
            StreamKey = r.Result.StreamKey;
            Url = r.Result.Url;
            StreamServerID = r.Result.StreamServerID;
            Usecs = r.Result.Usecs;
            Warning = r.Result.Warning; 
        }
        public string StreamKey { get; set; }

        public string Url { get; set; }

        public int StreamServerID { get; set; }

        public int Usecs { get; set; }

        public string Warning { get; set; }
    }
}
