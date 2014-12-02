using System.Runtime.Serialization;
using GrooveSharkClient.Models.Entity;
using GrooveSharkClient.Models.Exception;

namespace GrooveSharkClient.Models
{
    [DataContract]
    public class GrooveSharkResult
    {
        [DataMember(Name = "header")]
        public Header Header { get; set; }
        [DataMember(Name = "result")]
        public Result Result { get; set; }
        [DataMember(Name = "errors")]
        public GrooveSharkException[] Errors { get; set; }
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
        public bool? IsPlus { get; set; }

        [DataMember(Name = "IsAnywhere")]
        public bool? IsAnywhere { get; set; }

        [DataMember(Name = "IsPremium")]
        public bool? IsPremium { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "songs")]
        public Song[] Songs { get; set; }

        [DataMember(Name = "playlists")]
        public Playlist[] Playlists { get; set; }

        [DataMember(Name = "artists")]
        public Artist[] Artists { get; set; }

        [DataMember(Name = "albums")]
        public Album[] Albums { get; set; }

        #region Playlists Infos

        [DataMember(Name = "PlaylistName")]
        public string PlaylistName { get; set; }

        [DataMember(Name = "TSModified")]
        public int LastModificationTimeSpan { get; set; }

        [DataMember(Name = "PlaylistDescription")]
        public string PlaylistDescription { get; set; }

        [DataMember(Name = "CoverArtFilename")]
        public string CoverArtFilename { get; set; }

        #endregion

        #region CountryInfos

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

        #endregion

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
