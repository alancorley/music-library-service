using SongLibrary.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SongLibrary.API.Models
{
    public class SongDto
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Genre { get; set; }
        public string Filename { get; set; }

        public Guid ArtistID { get; set; }
        public ArtistDto Artist { get; set; }
    }
}
