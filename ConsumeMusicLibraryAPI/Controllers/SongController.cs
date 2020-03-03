using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MusicClient.Models;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using SongLibrary.API.Models;
using System.IO;
using MusicClient.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;

namespace MusicClient.Controllers
{

    public class SongController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SongController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> List(Guid Id)
        {
            IEnumerable<SongDto> model = await GetSongsByArtist(Id);
            if (!model.Any())
            {
                ArtistDto artist = await GetArtist(Id);
                if (artist != null)
                {
                    ViewBag.Artist = artist;
                }

            }
            return View(model);
        }

        public async Task<IActionResult> Create(Guid Id)
        {
            ArtistDto artist = await GetArtist(Id);
            if (artist != null)
            {
                ViewBag.ArtistId = artist.Id;
            }
            return View();
        }

        public async Task<IActionResult> Edit(Guid artistId, Guid songId)
        {
            if (artistId == Guid.Empty || songId == Guid.Empty)
            {
                return NotFound();
            }

            ViewBag.ArtistId = artistId;
            ViewBag.SongId = songId;

            var song = await GetSong(artistId, songId);
            if (song == null)
                return NotFound();
            return View(song);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SongForUpdateDto song, Guid artistId, Guid songId, SongViewModel model)
        {
            if (song != null && ModelState.IsValid)
            {
                // Get an instance of HttpClient from the factpry that we registered in Startup.cs
                var client = _httpClientFactory.CreateClient("API Client");

                var serializedArtistToEdit = JsonConvert.SerializeObject(song);

                var request = new HttpRequestMessage(HttpMethod.Put, "api/artists/" + artistId.ToString() + "/songs/" + songId.ToString());
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                request.Content = new StringContent(serializedArtistToEdit);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = await client.SendAsync(request);

                if (model.Filename != null && model.Filename.ContentType == "audio/mp3")
                {
                    //Upload audio file to API and returns the path to the file
                    string fileUploadPath = await UploadFile(model.Filename, songId.ToString());

                    //Need to update the song with the uploaded file's path
                    if (!string.IsNullOrWhiteSpace(fileUploadPath))
                    {
                        var patchDoc = new JsonPatchDocument<SongForUpdateDto>();
                        patchDoc.Replace(m => m.Filename, fileUploadPath);
                        await PatchSong(artistId.ToString(), songId.ToString(), patchDoc);
                    }
                }


                response.EnsureSuccessStatusCode();

                return RedirectToAction("list", "song", new { id = artistId.ToString() });
            }
            return View(song);
        }

        public async Task<IActionResult> Delete(Guid artistId, Guid songId)
        {
            if (artistId == null || songId == null)
            {
                return NotFound();
            }
            // Get an instance of HttpClient from the factpry that we registered in Startup.cs
            var client = _httpClientFactory.CreateClient("API Client");

            var request = new HttpRequestMessage(HttpMethod.Delete, "api/artists/" + artistId.ToString() + "/songs/" + songId.ToString());
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return RedirectToAction("list", "song", new { id = artistId.ToString() });
        }

        [HttpPost]
        public async Task<IActionResult> Create(SongForCreationDto song, Guid artistId, SongViewModel model)
        {

            if (song != null && ModelState.IsValid)
            {
                //Create the new instance of the song 
                var client = _httpClientFactory.CreateClient("API Client");

                var serializedSongToCreate = JsonConvert.SerializeObject(song);

                var request = new HttpRequestMessage(HttpMethod.Post, "api/artists/" + artistId.ToString() + "/songs");
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                request.Content = new StringContent(serializedSongToCreate);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = await client.SendAsync(request);
                var responsePayload = await response.Content.ReadAsStringAsync();
                var responsePayloadDeserialized = JsonConvert.DeserializeObject<SongDto>(responsePayload);
                string songId = responsePayloadDeserialized.Id.ToString();

                if(model.Filename != null && model.Filename.ContentType == "audio/mp3")
                {
                    //Upload audio file to API and returns the path to the file
                    string fileUploadPath = await UploadFile(model.Filename, songId);

                    //Need to update the song with the uploaded file's path
                    if (!string.IsNullOrWhiteSpace(fileUploadPath))
                    {
                        var patchDoc = new JsonPatchDocument<SongForUpdateDto>();
                        patchDoc.Replace(m => m.Filename, fileUploadPath);
                        await PatchSong(artistId.ToString(), songId, patchDoc);
                    }
                }


                response.EnsureSuccessStatusCode();
                return RedirectToAction("list", "song", new { id = artistId.ToString() });

            }
            return View(song);
        }

        private async Task PatchSong(string artistId, string songId, JsonPatchDocument<SongForUpdateDto> patchDoc)
        {
            var serializedChangeSet = JsonConvert.SerializeObject(patchDoc);

            var request = new HttpRequestMessage(HttpMethod.Patch, "api/artists/" + artistId + "/songs/" + songId);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = new StringContent(serializedChangeSet);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json-patch+json");

            var client = _httpClientFactory.CreateClient("API Client");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
        private async Task<SongForUpdateDto> GetSong(Guid artistId, Guid songID)
        {
            // Get an instance of HttpClient from the factpry that we registered in Startup.cs
            var client = _httpClientFactory.CreateClient("API Client");

            var result = await client.GetAsync("/api/artists/" + artistId.ToString() + "/songs/" + songID.ToString());

            if (result.IsSuccessStatusCode)
            {
                // Read all of the response and deserialise it into an instace of
                var content = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<SongForUpdateDto>(content);
            }
            return null;
        }

        private async Task<IEnumerable<SongDto>> GetSongsByArtist(Guid artistID)
        {
            // Get an instance of HttpClient from the factpry that we registered in Startup.cs
            var client = _httpClientFactory.CreateClient("API Client");

            var result = await client.GetAsync("/api/artists/" + artistID + "/songs");

            if (result.IsSuccessStatusCode)
            {
                // Read all of the response and deserialise it into an instace of
                var content = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<SongDto>>(content);
            }
            return null;
        }

        private async Task<ArtistDto> GetArtist(Guid Id)
        {
            // Get an instance of HttpClient from the factpry that we registered in Startup.cs
            var client = _httpClientFactory.CreateClient("API Client");

            var result = await client.GetAsync("/api/artists/" + Id.ToString());

            if (result.IsSuccessStatusCode)
            {
                // Read all of the response and deserialise it into an instace of
                var content = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ArtistDto>(content);
            }
            return null;
        }

        public async Task<IActionResult> Download(Guid artistId, Guid songId)
        {
            //TODO: NEED TO FIX THIS TO USE API
            return NotFound();

            if (artistId == Guid.Empty || songId == Guid.Empty)
            {
                return NotFound();
            }
            var song = await GetSong(artistId, songId);
            if (song == null)
                return NotFound();


            if (string.IsNullOrWhiteSpace(song.Filename))
                return NotFound();

            var path = Path.Combine(
                           Directory.GetCurrentDirectory(),
                           "wwwroot", song.Filename);

            if (!string.IsNullOrWhiteSpace(path))
            {
                // Get an instance of HttpClient from the factory that we registered in Startup.cs
                var client = _httpClientFactory.CreateClient("API Client");

                var result = await client.GetAsync("/api/files/");

                //HttpContent fileStreamContent = new StreamContent(file.OpenReadStream());
                //fileStreamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data") { Name = "file", FileName = file.FileName };
                //fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue("audio/mpeg"); //TODO: remove hardcoded contenttype and make dynamic
                //using (var formData = new MultipartFormDataContent())
                //{
                //    formData.Add(fileStreamContent);
                //    var uploadReponse = await client.PostAsync("api/files/" + songId, formData);
                //    var uploadReponsePayload = uploadReponse.Content.ReadAsStringAsync().Result;
                //    dynamic data = JsonConvert.DeserializeObject(uploadReponse.Content.ReadAsStringAsync().Result);
                //    string newFilePath = data.message;
                //    return newFilePath;
                //}
            }


            //var memory = new MemoryStream();
            //using (var stream = new FileStream(path, FileMode.Open))
            //{
            //    await stream.CopyToAsync(memory);
            //}
            //memory.Position = 0;
            //return File(memory, "audio/mpeg", Path.GetFileName(path));
        }

        private async Task<string> UploadFile(IFormFile file, string songId)
        {
            //Upload audio file to API 
            if (file != null || file.FileName != null || file.FileName.Length != 0)
            {
                // Get an instance of HttpClient from the factory that we registered in Startup.cs
                var client = _httpClientFactory.CreateClient("API Client");

                HttpContent fileStreamContent = new StreamContent(file.OpenReadStream());
                fileStreamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data") { Name = "file", FileName = file.FileName };
                fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue("audio/mpeg"); //TODO: remove hardcoded contenttype and make dynamic
                using (var formData = new MultipartFormDataContent())
                {
                    formData.Add(fileStreamContent);
                    var uploadReponse = await client.PostAsync("api/files/" + songId, formData);
                    var uploadReponsePayload = uploadReponse.Content.ReadAsStringAsync().Result;
                    dynamic data = JsonConvert.DeserializeObject(uploadReponse.Content.ReadAsStringAsync().Result);
                    string newFilePath = data.message;
                    return newFilePath;
                }
            }
            return "";
        }
        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}