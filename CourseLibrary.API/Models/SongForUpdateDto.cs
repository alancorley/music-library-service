using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SongLibrary.API.Models
{
    public class SongForUpdateDto : SongForManipulationDto
    {
        

        [Required(ErrorMessage = "You should fill out a genre.")]
        public override string Genre { get => base.Genre; set => base.Genre = value; }

    }
}
