using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicClient.ViewModel
{
    public class SongViewModel
    {
        public string Title { get; set; }
        public string Genre { get; set; }
        public IFormFile Filename { get; set; }
    }
}
