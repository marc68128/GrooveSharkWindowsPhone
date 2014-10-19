using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrooveSharkClient.Models
{
    public class Song
    {
        public int SongID { get; set; }
        public string SongName { get; set; }
        public int ArtistID { get; set; }
        public string ArtistName { get; set; }
        public int AlbumID { get; set; }
        public string AlbumName { get; set; }
        public string CoverArtFilename { get; set; }
        public string Popularity { get; set; }
        public bool IsLowBitrateAvailable { get; set; }
        public bool IsVerified { get; set; }
        public int Flags { get; set; }
    }
}
