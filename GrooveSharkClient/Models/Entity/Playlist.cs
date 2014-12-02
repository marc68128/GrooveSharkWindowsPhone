namespace GrooveSharkClient.Models.Entity
{
    public class Playlist
    {
        public Playlist()
        {
            
        }
        public Playlist(GrooveSharkResult result)
        {
            PlaylistName = result.Result.PlaylistName;
            UserID = result.Result.UserID;
            FName = result.Result.FName;
            LName = result.Result.LName;
            PlaylistName = result.Result.PlaylistName;
            LastModificationTimeSpan = result.Result.LastModificationTimeSpan;
            CoverArtFilename = result.Result.CoverArtFilename;
            PlaylistDescription = result.Result.PlaylistDescription;
            Songs = result.Result.Songs;
        }

        public int PlaylistID { get; set; }
        public int UserID { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string PlaylistName { get; set; }
        public string TSAdded { get; set; }
        public int LastModificationTimeSpan { get; set; }
        public string PlaylistDescription { get; set; }
        public string CoverArtFilename { get; set; }

        public Song[] Songs { get; set; }

        public override string ToString()
        {
            return string.Format("[Playlist] Name : {0} - User : {1}", PlaylistName, FName);
        }
    }
}
