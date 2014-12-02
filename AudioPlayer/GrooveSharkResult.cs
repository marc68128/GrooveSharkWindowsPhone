using System;
using System.Runtime.Serialization;

namespace AudioPlayer
{
    [DataContract]
    public sealed class GrooveSharkResult
    {
        [DataMember(Name = "header")]
        public Header Header { get; set; }
        [DataMember(Name = "result")]
        public Result Result { get; set; }
        [DataMember(Name = "errors")]
        public Exception[] Errors { get; set; }
    }

    [DataContract]
    public sealed class Header
    {
        [DataMember(Name = "hostname")]
        public string Hostname { get; set; }
    }

    [DataContract]
    public sealed class Result
    {
        [DataMember(Name = "success")]
        public bool Success { get; set; }

        #region StreamInfos

        [DataMember(Name = "StreamKey")]
        public string StreamKey { get; set; }

         [DataMember(Name = "url")]
        public string Url { get; set; }

         [DataMember(Name = "StreamServerID")]
        public int StreamServerID { get; set; }

         [DataMember(Name = "uSecs")]
        public int Usecs { get; set; }

         [DataMember(Name = "warning")]
        public string Warning { get; set; }

        #endregion
    }
}
