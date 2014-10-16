using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrooveSharkClient.Models
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
            IsPlus = r.Result.IsPlus;
            IsAnywhere = r.Result.IsAnywhere;
            IsPremium = r.Result.IsPremium;
        }

        public int UserID { get; set; }
        public string Email { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public bool IsPlus { get; set; }
        public bool IsAnywhere { get; set; }
        public bool IsPremium { get; set; }
    }
}
