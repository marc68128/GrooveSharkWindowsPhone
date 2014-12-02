using System.Runtime.Serialization;

namespace GrooveSharkClient.Models.Exception
{
    public class GrooveSharkException : System.Exception
    {
        [DataMember(Name = "code")]
        public int Code { get; set; }

        [DataMember(Name = "message")]
        public string Description { get; set; }

        public override string Message
        {
            get { return Description; }
        }

        public GrooveSharkException(System.Exception e) : base("", e)
        {
            Description = e.Message;
        }

        public GrooveSharkException()
        {
            
        }

    }

}
