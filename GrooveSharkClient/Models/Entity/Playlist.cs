using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrooveSharkClient.Models.Entity
{
    public class Playlist
    {
        public int PlaylistID { get; set; }
        public int UserID { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string PlaylistName { get; set; }
        public string TSAdded { get; set; }

        public override string ToString()
        {
            return string.Format("[Playlist] Name : {0} - User : {1}", PlaylistName, FName);
        }
    }
}
