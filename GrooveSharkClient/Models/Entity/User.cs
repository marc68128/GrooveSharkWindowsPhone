namespace GrooveSharkClient.Models.Entity
{
    public class User
    {
        public User()
        {
            
        }

        public User(GrooveSharkResult r)
        {
            UserID = r.Result.UserID;
            Email = r.Result.Email;
            FName = r.Result.FName;
            LName = r.Result.LName;
            IsPlus = r.Result.IsPlus.HasValue && r.Result.IsPlus.Value;
            IsAnywhere = r.Result.IsAnywhere.HasValue && r.Result.IsAnywhere.Value;
            IsPremium = r.Result.IsPremium.HasValue && r.Result.IsPremium.Value;
        }

        public int UserID { get; set; }
        public string Email { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public bool IsPlus { get; set; }
        public bool IsAnywhere { get; set; }
        public bool IsPremium { get; set; }

        public override string ToString()
        {
            return "UserId : " + UserID;
        }
    }
}
