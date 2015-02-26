using GrooveSharkClient.Helpers;
using Newtonsoft.Json;

namespace GrooveSharkClient.Models.Entity
{
    public class Song 
    {
        public int SongID { get; set; }
        public string SongName { get; set; }
        public string Name { get; set; }
        public int ArtistID { get; set; }
        public string ArtistName { get; set; }
        public int AlbumID { get; set; }
        public string AlbumName { get; set; }
        public string CoverArtFilename { get; set; }
        public string Popularity { get; set; }

        [JsonConverter(typeof(BoolConverter))]
        public bool IsLowBitrateAvailable { get; set; }

        [JsonConverter(typeof(BoolConverter))]
        public bool IsVerified { get; set; }
        public int Flags { get; set; }
        public int Sort { get; set; }
    }
}
