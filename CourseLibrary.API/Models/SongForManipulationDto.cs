using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SongLibrary.API.Models

    //SUMMARY: Consolidating properties shared between Create and Update DTOs to reduce redundant code; The Create and Update DTOs will inherit this DTO
{
    public abstract class SongForManipulationDto
    {
        [Required(ErrorMessage = "You should fill out a title.")]
        [MaxLength(100, ErrorMessage = "The title shouldn't have more than 100 characters.")]
        public string Title { get; set; }

        [MaxLength(100, ErrorMessage = "The genre shouldn't have more than 100 characters.")]
        public virtual string Genre { get; set; }

        [MaxLength(100, ErrorMessage = "The filename shouldn't be more than 100 characters.")]
        public virtual string Filename { get; set; }


        //[Required(ErrorMessage = "Artist is required.")]
        //public virtual ArtistDto Artist { get; set; }
    }
}
