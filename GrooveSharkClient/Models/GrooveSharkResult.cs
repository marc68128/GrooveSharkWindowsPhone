﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using GrooveSharkClient.Models.Entity;

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

        [DataMember(Name = "songs")]
        public Song[] Songs { get; set; }

        [DataMember(Name = "playlists")]
        public Playlist[] Playlists { get; set; }

        [DataMember(Name = "artists")]
        public Artist[] Artists { get; set; }

        [DataMember(Name = "albums")]
        public Album[] Albums { get; set; }

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

    }



}
