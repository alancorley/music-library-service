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
using SongLibrary.API.Services;

namespace MusicClient.Controllers
{
    public class ArtistController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ArtistController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var model = await GetArtists();
            return View(model);
        }

        public async Task<IActionResult> Edit(Guid Id)  
        {
            var artist = await GetArtistForUpdate(Id);
            if (artist == null)
                return NotFound();
            return View(artist);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ArtistForCreationDto artist)
        {
            if (artist != null && ModelState.IsValid)
            {
                // Get an instance of HttpClient from the factpry that we registered in Startup.cs
                var client = _httpClientFactory.CreateClient("API Client");

                var serializedArtistToCreate = JsonConvert.SerializeObject(artist);

                var request = new HttpRequestMessage(HttpMethod.Post, "api/artists");
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                request.Content = new StringContent(serializedArtistToCreate);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                return RedirectToAction("Index");
            }
            return View(artist);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ArtistForUpdateDto artist)
        {
            if (artist != null && ModelState.IsValid)
            {
                // Get an instance of HttpClient from the factpry that we registered in Startup.cs
                var client = _httpClientFactory.CreateClient("API Client");

                var serializedArtistToEdit = JsonConvert.SerializeObject(artist);

                var request = new HttpRequestMessage(HttpMethod.Put, "api/artists/" + artist.Id.ToString());
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                request.Content = new StringContent(serializedArtistToEdit);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                return RedirectToAction("Index");
            }
            return View(artist);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //First, get list of songs associated to this Artist and delete the associated audio files
            IEnumerable<SongDto> songsForArtist = await GetSongsByArtist(id);
            if (songsForArtist.Any())
            {
                foreach (SongDto song in songsForArtist)
                {
                    DeleteFile(song.Id);
                }
            }

            //Delete Artist (Song child rows will be deleted automatically)
            await DeleteArtistAsync(id);

            return RedirectToAction("Index");
        }



        private async Task<IEnumerable<ArtistDto>> GetArtists()
        {
            // Get an instance of HttpClient from the factpry that we registered in Startup.cs
            var client = _httpClientFactory.CreateClient("API Client");

            var result = await client.GetAsync("/api/artists");

            if (result.IsSuccessStatusCode)
            {
                // Read all of the response and deserialise it into an instace of
                var content = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<ArtistDto>>(content);
            }
            return null;
        }

        private async Task<ArtistForUpdateDto> GetArtistForUpdate(Guid Id)
        {
            // Get an instance of HttpClient from the factpry that we registered in Startup.cs
            var client = _httpClientFactory.CreateClient("API Client");

            var result = await client.GetAsync("/api/artists/" + Id.ToString());

            if (result.IsSuccessStatusCode)
            {
                // Read all of the response and deserialise it into an instace of
                var content = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ArtistForUpdateDto>(content);
            }
            return null;
        }

        private void DeleteFile(Guid songId)
        {
            //delete audio file
            var client = _httpClientFactory.CreateClient("API Client");

            var request = new HttpRequestMessage(HttpMethod.Delete, "api/files/" + songId.ToString());
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            client.SendAsync(request);
        }

        private async Task DeleteArtistAsync(Guid artistId)
        {
            var client = _httpClientFactory.CreateClient("API Client");

            var request = new HttpRequestMessage(HttpMethod.Delete, "api/artists/" + artistId.ToString());
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
