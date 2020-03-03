using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SongLibrary.API.Entities
{
    public class Song
    {
        [Key]       
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(100)]
        public string Genre { get; set; }

        [MaxLength(500)]
        public string Filename { get; set; }

        [ForeignKey("ArtistId")]
        public Guid ArtistId { get; set; }

        public Artist Artist { get; set; }
    }
}