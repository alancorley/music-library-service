using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SongLibrary.API.Models
{
    public class ArtistDto
    {
        public Guid Id { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Artist")]
        public string FullName
        {
            get { return FirstName + " " + LastName; }
        }

        //public ICollection<SongDto> Songs { get; set; }
        //= new List<SongDto>();

    }
}
