using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SongLibrary.API.Models
{
    public class ArtistDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        //public string LastName { get; set; }

        //public ICollection<SongDto> Songs { get; set; }
            //= new List<SongDto>();

    }
}
