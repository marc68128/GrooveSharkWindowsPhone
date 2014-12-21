using System;
using System.Net;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using Windows.UI.Popups;
using Newtonsoft.Json;
using ReactiveUI;

namespace AudioPlayer
{
    [DataContract]
    [JsonObject(MemberSerialization.OptOut)]
    public sealed class SongViewModel
    {
        public int SongId { get; set; }
        public string ArtistName { get; set; }
        public string SongName { get; set; }
        public string AlbumName { get; set; }
        public string ThumbnailUrl { get; set; }
        [JsonIgnore]
        public int StreamUsecs { get; set; }
        [JsonIgnore]
        public int StreamServerId { get; set; }
        [JsonIgnore]
        public string StreamKey { get; set; }
        [JsonIgnore]
        public string StreamUrl { get; set; }

        public static SongViewModel Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<SongViewModel>(json);
        }
        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

}
