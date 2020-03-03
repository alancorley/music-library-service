using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SongLibrary.API.Entities
{
    public class Artist
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        public ICollection<Song> Songs { get; set; }
            = new List<Song>();
    }
}
