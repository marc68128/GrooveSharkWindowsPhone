namespace AudioPlayer
{
    public sealed class StreamInfo
    {
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
