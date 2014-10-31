using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrooveSharkClient.Models.Entity
{
    public class Album
    {
        public int AlbumID { get; set; }
        public string AlbumName { get; set; }
        public int ArtistID { get; set; }
        public string ArtistName { get; set; }
        public string CoverArtFilename { get; set; }
        public bool IsVerified { get; set; }

        public override string ToString()
        {
            return string.Format("[Album] Name : {0} - ArtistName : {1}", AlbumName, ArtistName);
        }
    }
}
