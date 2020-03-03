using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SongLibrary.API.Models
{
    public class ArtistForCreationDto
    {
        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; }


    }
}
