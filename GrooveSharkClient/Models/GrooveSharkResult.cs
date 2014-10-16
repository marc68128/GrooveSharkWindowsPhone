using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GrooveSharkClient.Models
{
    [DataContract]
    public class GrooveSharkResult
    {
        [DataMember(Name = "header")]
        public Header Header { get; set; }
        [DataMember(Name = "result")]
        public Result Result { get; set; }

    }

    [DataContract]
    public class Header
    {
        [DataMember(Name = "hostname")]
        public string Hostname { get; set; }
    }

    [DataContract]
    public class Result
    {
        [DataMember(Name = "success")]
        public bool Success { get; set; }

        [DataMember(Name = "sessionID")]
        public string SessionID { get; set; }

        [DataMember(Name = "UserID")]
        public int UserID { get; set; }

        [DataMember(Name = "Email")]
        public string Email { get; set; }

        [DataMember(Name = "FName")]
        public string FName { get; set; }

        [DataMember(Name = "LName")]
        public string LName { get; set; }

        [DataMember(Name = "IsPlus")]
        public bool IsPlus { get; set; }

        [DataMember(Name = "IsAnywhere")]
        public bool IsAnywhere { get; set; }

        [DataMember(Name = "IsPremium")]
        public bool IsPremium { get; set; }


    }





}
