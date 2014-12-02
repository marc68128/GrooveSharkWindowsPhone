using System;
using System.Net;
using System.Reactive.Linq;
using Windows.UI.Popups;
using GrooveSharkClient.Models;
using GrooveSharkClient.Models.Entity;
using GrooveSharkClient.Models.Exception;
using Newtonsoft.Json;
using ReactiveUI;

namespace AudioPlayer
{
    public sealed class SongViewModel
    {
        public int SongId { get; set; }
        public string ArtistName { get; set; }
        public string SongName { get; set; }
        public string AlbumName { get; set; }
        public string ThumbnailUrl { get; set; }
        public int StreamUsecs { get; set; }
        public int StreamServerId { get; set; }
        public string StreamKey { get; set; }
        public string StreamUrl { get; set; }

        public static SongViewModel Deserialize(string json)
        {
            var splited = json.Replace("\"", "").Split(';');
            return new SongViewModel() {
                SongName = splited[0],
                SongId = int.Parse(splited[1]),
                AlbumName = splited[2],
                ArtistName = splited[3],
                ThumbnailUrl = splited[4],
                StreamUrl = splited[5],
                StreamKey = splited[6],
                StreamServerId = int.Parse(splited[7]),
                StreamUsecs = int.Parse(splited[8])
            };
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(SongName + ";" + SongId + ";" + AlbumName + ";" + ArtistName + ";" + ThumbnailUrl + ";" + StreamUrl + ";" + StreamKey + ";" + StreamServerId + ";" + StreamUsecs);
        }
    }

}
