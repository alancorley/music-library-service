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
        const string uploadDirectory = "\\Upload";
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
                if (file.Length > 0 && file.ContentType.ToString() == "audio/mpeg") //TODO: RESTRICTED TO MPEG: AT SOME POINT, INCLUDE ADDL FILE EXTENSIONS
                {
                    if (!Directory.Exists(_environment.WebRootPath + uploadDirectory))
                    {
                        Directory.CreateDirectory(_environment.WebRootPath + uploadDirectory);
                    }

                    string filePath = _environment.WebRootPath + uploadDirectory + "\\" + songId.ToString() + ".mp3";
                    using (FileStream fileStream = System.IO.File.Create(filePath))
                    {
                        await file.CopyToAsync(fileStream);
                        await fileStream.FlushAsync();
                        return Ok(new { message = filePath.ToString() });
                    }
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        public IActionResult Download([FromQuery(Name = "filepath")] string filepath)
        {
            try
            {
                //check for existance of requested file and return if exists
                string filePath = Path.Combine(_environment.WebRootPath, filepath);
                if (System.IO.File.Exists(filePath))
                {
                    return PhysicalFile(filePath, "application/octet-stream", true);
                }
                
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            try
            {
                //check for existance of requested file and return if exists
                string filePath = _environment.WebRootPath + uploadDirectory + "\\" + id + ".mp3";
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    return Ok();
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

    }
}
