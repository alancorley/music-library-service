using AutoMapper;
using SongLibrary.API.Models;
using SongLibrary.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Http;

namespace SongLibrary.API.Controllers
{
    [ApiController]
    [Route("api/artists/{artistId}/songs")]
    public class SongController : ControllerBase
    {
        private readonly ISongLibraryRepository _songLibraryRepository;
        private readonly IMapper _mapper;
        

        public SongController(ISongLibraryRepository songLibraryRepository,
            IMapper mapper)
        {
            _songLibraryRepository = songLibraryRepository ??
                throw new ArgumentNullException(nameof(songLibraryRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));

            
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SongDto>>> GetSongsForArtist(Guid artistId)
        {
            if (!_songLibraryRepository.ArtistExists(artistId))
            {
                return NotFound();
            }

            var songsForArtistFromRepo = await _songLibraryRepository.GetSongsAsync(artistId);
            return Ok(_mapper.Map<IEnumerable<SongDto>>(songsForArtistFromRepo));
        }

        [HttpGet("{songId}", Name = "GetSongForArtist")]
        public async Task<ActionResult<SongDto>> GetSongForArtist(Guid artistId, Guid songId)
        {
            if (!_songLibraryRepository.ArtistExists(artistId))
            {
                return NotFound();
            }

            var songForArtistFromRepo = await _songLibraryRepository.GetSongAsync(artistId, songId);

            if (songForArtistFromRepo == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<SongDto>(songForArtistFromRepo));
        }

        [HttpPost]
        public async Task<ActionResult<SongDto>> CreateSongForArtist(
            Guid artistId, SongForCreationDto song)
        {
            if (!_songLibraryRepository.ArtistExists(artistId))
            {
                return NotFound();
            }

            var songEntity = _mapper.Map<Entities.Song>(song);
            await _songLibraryRepository.AddSongAsync(artistId, songEntity);
            await _songLibraryRepository.SaveChangesAsync();

            var songToReturn = _mapper.Map<SongDto>(songEntity);
            return CreatedAtRoute("GetSongForArtist",
                new { artistId = artistId, songId = songToReturn.Id }, 
                songToReturn);
        }

        [HttpPut("{songId}")]
        public async Task<IActionResult> UpdateSongForArtist(Guid artistId, 
            Guid songId, 
            SongForUpdateDto song)
        {
            if (!_songLibraryRepository.ArtistExists(artistId))
            {
                return NotFound();
            }

            var songForArtistFromRepo = await _songLibraryRepository.GetSongAsync(artistId, songId);

            if (songForArtistFromRepo == null)
            {
                //if doesn't exist, then create it
                var songToAdd = _mapper.Map<Entities.Song>(song);
                songToAdd.Id = songId;

                await _songLibraryRepository.AddSongAsync(artistId, songToAdd);

                await _songLibraryRepository.SaveChangesAsync();

                var songToReturn = _mapper.Map<SongDto>(songToAdd);

                return CreatedAtRoute("GetSongForArtist",
                    new { artistId, songId = songToReturn.Id },
                    songToReturn);
            }

            // map the entity to a SongForUpdateDto
            // apply the updated field values to that dto
            // map the SongForUpdateDto back to an entity
            _mapper.Map(song, songForArtistFromRepo);

            _songLibraryRepository.UpdateSong(songForArtistFromRepo);

            await _songLibraryRepository.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{songId}")]
        public async Task<ActionResult> PartiallyUpdateSongForArtist(Guid artistId, 
            Guid songId,
            JsonPatchDocument<SongForUpdateDto> patchDocument)
        {
            if (!_songLibraryRepository.ArtistExists(artistId))
            {
                return NotFound();
            }

            var songForArtistFromRepo = await _songLibraryRepository.GetSongAsync(artistId, songId);

            if (songForArtistFromRepo == null)
            {
                var songDto = new SongForUpdateDto();
                patchDocument.ApplyTo(songDto, ModelState);

                if (!TryValidateModel(songDto))
                {
                    return ValidationProblem(ModelState);
                }

                var songToAdd = _mapper.Map<Entities.Song>(songDto);
                songToAdd.Id = songId;

                await _songLibraryRepository.AddSongAsync(artistId, songToAdd);
                await _songLibraryRepository.SaveChangesAsync();

                var songToReturn = _mapper.Map<SongDto>(songToAdd);

                return CreatedAtRoute("GetSongForArtist",
                    new { artistId, songId = songToReturn.Id }, 
                    songToReturn);
            }

            var songToPatch = _mapper.Map<SongForUpdateDto>(songForArtistFromRepo);
            // add validation
            patchDocument.ApplyTo(songToPatch, ModelState);

            if (!TryValidateModel(songToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(songToPatch, songForArtistFromRepo);

            _songLibraryRepository.UpdateSong(songForArtistFromRepo);

            _songLibraryRepository.Save();

            return NoContent();
        }

        [HttpDelete("{songId}")]
        public async Task<ActionResult> DeleteSongForArtist(Guid artistId, Guid songId)
        {
            if (!_songLibraryRepository.ArtistExists(artistId))
            {
                return NotFound();
            }

            var songForArtistFromRepo = await _songLibraryRepository.GetSongAsync(artistId, songId);

            if (songForArtistFromRepo == null)
            {
                return NotFound();
            }

            _songLibraryRepository.DeleteSong(songForArtistFromRepo);
            await _songLibraryRepository.SaveChangesAsync();

            return NoContent();
        }

        public override ActionResult ValidationProblem(
            [ActionResultObjectValue] ModelStateDictionary modelStateDictionary)
        {
            var options = HttpContext.RequestServices
                .GetRequiredService<IOptions<ApiBehaviorOptions>>();
            return (ActionResult)options.Value.InvalidModelStateResponseFactory(ControllerContext);
        }
    }
}