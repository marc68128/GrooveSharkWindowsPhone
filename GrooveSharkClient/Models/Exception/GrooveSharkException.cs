using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GrooveSharkClient.Models
{
    public class GrooveSharkException : Exception
    {
        [DataMember(Name = "code")]
        public int Code { get; set; }

        [DataMember(Name = "message")]
        public string Description { get; set; }

        public override string Message
        {
            get { return Description; }
        }

        public GrooveSharkException(Exception e) : base("", e)
        {
            Description = e.Message;
        }

        public GrooveSharkException()
        {
            
        }

    }

}
