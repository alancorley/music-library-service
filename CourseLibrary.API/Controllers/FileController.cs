using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SongLibrary.API.Controllers
{
    [Route("api/files")]
    public class FileController : Controller
    {
        public static IWebHostEnvironment _environment;

        public FileController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpPost("{songId}")]
        public async Task<IActionResult> Upload([FromForm(Name = "file")] IFormFile file, Guid songId)
        {
            try
            {
                if (file.Length > 0 && file.ContentType.ToString() == "audio/mpeg") //RESTRICTED TO MPEG: AT SOME POINT, INCLUDE ADDL FILE EXTENSIONS
                {
                    if (!Directory.Exists(_environment.WebRootPath + "\\Upload"))
                    {
                        Directory.CreateDirectory(_environment.WebRootPath + "\\Upload\\");
                    }
                    string filePath = "\\Upload\\" + songId.ToString() + file.FileName.ToLower();
                    using (FileStream fileStream = System.IO.File.Create(_environment.WebRootPath + filePath))
                    {
                        await file.CopyToAsync(fileStream);
                        await fileStream.FlushAsync();
                        return Ok(new { message = filePath.ToString() });
                    }
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}
