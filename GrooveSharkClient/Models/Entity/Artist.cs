namespace GrooveSharkClient.Models.Entity
{
    public class Artist
    {
        public int ArtistID { get; set; }
        public string ArtistName { get; set; }
        public bool IsVerified { get; set; }

        public override string ToString()
        {
            return string.Format("[Artist] Name : {0}", ArtistName);
        }
    }
}
