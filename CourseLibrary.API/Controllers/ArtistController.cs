using AutoMapper;
using SongLibrary.API.Helpers;
using SongLibrary.API.Models;
using SongLibrary.API.ResourceParameters;
using SongLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SongLibrary.API.Controllers
{
    [ApiController]
    [Route("api/artists")]
    public class ArtistController : ControllerBase
    {
        private readonly ISongLibraryRepository _songLibraryRepository;
        private readonly IMapper _mapper;

        public ArtistController(ISongLibraryRepository songLibraryRepository,
            IMapper mapper)
        {
            _songLibraryRepository = songLibraryRepository ??
                throw new ArgumentNullException(nameof(songLibraryRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet()]
        [HttpHead]
        public async Task<ActionResult<IEnumerable<ArtistDto>>> GetArtists(
            [FromQuery] ArtistsResourceParameters artistsResourceParameters)
        {
            var artistsFromRepo = await _songLibraryRepository.GetArtistsAsync(artistsResourceParameters);
            return Ok(_mapper.Map<IEnumerable<ArtistDto>>(artistsFromRepo));
        }

        [HttpGet("{artistId}", Name ="GetArtist")]
        public IActionResult GetArtist(Guid artistId)
        {
            var artistFromRepo = _songLibraryRepository.GetArtist(artistId);

            if (artistFromRepo == null)
            {
                return NotFound();
            }
             
            return Ok(_mapper.Map<ArtistDto>(artistFromRepo));
        }

        [HttpPut("{artistId}")]
        public async Task<IActionResult> UpdateArtist(Guid artistId,
            ArtistForUpdateDto artist)
        {
            if (!_songLibraryRepository.ArtistExists(artistId))
            {
                return NotFound();
            }

            var artistFromRepo =  _songLibraryRepository.GetArtist(artistId);

            if (artistFromRepo == null)
            {
                //if doesn't exist, then create it
                var artistToAdd = _mapper.Map<Entities.Artist>(artist);
                artistToAdd.Id = artistId;

                 _songLibraryRepository.AddArtist(artistToAdd);

                await _songLibraryRepository.SaveChangesAsync();

                var artistToReturn = _mapper.Map<ArtistDto>(artistToAdd);

                return CreatedAtRoute("GetArtist",
                    new { artistId, songId = artistToReturn.Id },
                    artistToReturn);
            }

            // map the entity to a ArtistForUpdateDto
            // apply the updated field values to that dto
            // map the ArtistForUpdateDto back to an entity
            _mapper.Map(artist, artistFromRepo);

            _songLibraryRepository.UpdateArtist(artistFromRepo);

            await _songLibraryRepository.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<ArtistDto>> CreateArtistAsync([FromBody] ArtistForCreationDto artist)
        {
            var artistEntity = _mapper.Map<Entities.Artist>(artist);
            _songLibraryRepository.AddArtist(artistEntity);
            await _songLibraryRepository.SaveChangesAsync();

            var artistToReturn = _mapper.Map<ArtistDto>(artistEntity);
            return CreatedAtRoute("GetArtist",
                new { artistId = artistToReturn.Id },
                artistToReturn);
        }

        [HttpOptions]
        public IActionResult GetArtistsOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST");
            return Ok();
        }

        [HttpDelete("{artistId}")]
        public ActionResult DeleteArtist(Guid artistId)
        {
            var artistFromRepo = _songLibraryRepository.GetArtist(artistId);

            if (artistFromRepo == null)
            {
                return NotFound();
            }

            _songLibraryRepository.DeleteArtist(artistFromRepo);

            _songLibraryRepository.Save();

            return NoContent();
        }
    }
}
