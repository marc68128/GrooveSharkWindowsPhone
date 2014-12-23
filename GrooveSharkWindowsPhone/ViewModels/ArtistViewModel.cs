using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrooveSharkClient.Models.Entity;
using ReactiveUI;

namespace GrooveSharkWindowsPhone.ViewModels
{
    public class ArtistViewModel : BaseViewModel
    {
        public ArtistViewModel(Artist artist)
        {
            Id = artist.ArtistID;
            Name = artist.ArtistName;
        }

        private int _id;
        public int Id
        {
            get { return _id; }
            set { this.RaiseAndSetIfChanged(ref _id, value); }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { this.RaiseAndSetIfChanged(ref _name, value); }
        }
        

    }
}
