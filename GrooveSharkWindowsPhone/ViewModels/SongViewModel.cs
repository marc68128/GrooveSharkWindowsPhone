using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrooveSharkClient.Models;
using ReactiveUI;

namespace GrooveSharkWindowsPhone.ViewModels
{
    public class SongViewModel : ReactiveObject
    {
        private Song _s; 

        public SongViewModel(Song s, int position)
        {
            _s = s;
            SongPosition = position; 
            ThumbnailUrl = !string.IsNullOrEmpty(s.CoverArtFilename) ? "http://images.gs-cdn.net/static/albums/70_" + s.CoverArtFilename : "/Assets/Images/Songs/no_cover_63.png";
        }

        public string ArtistName { get { return _s.ArtistName;  } }
        public string SongName { get { return _s.SongName; } }
        public string AlbumName { get { return _s.AlbumName; } }

        private int _songPosition;
        public int SongPosition
        {
            get { return _songPosition; }
            set { this.RaiseAndSetIfChanged(ref _songPosition, value); }
        }
        

        private string _thumbnailUrl;
        public string ThumbnailUrl
        {
            get { return _thumbnailUrl; }
            set { this.RaiseAndSetIfChanged(ref _thumbnailUrl, value); }
        }
        
    }

}
