using SongLibrary.API.DbContexts;
using SongLibrary.API.Entities;
using SongLibrary.API.ResourceParameters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SongLibrary.API.Services
{
    public class SongLibraryRepository : ISongLibraryRepository, IDisposable 
    {
        private readonly SongLibraryContext _context;

        public SongLibraryRepository(SongLibraryContext context )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region "Songs"
            public async Task AddSongAsync(Guid artistId, Song song)
            {
                if (artistId == Guid.Empty)
                {
                    throw new ArgumentNullException(nameof(artistId));
                }

                if (song == null)
                {
                    throw new ArgumentNullException(nameof(song));
                }
                // always set the ArtistId to the passed-in artistId
                song.ArtistId = artistId;
                await _context.Songs.AddAsync(song);
            }

            public void DeleteSong(Song song)
            {
                _context.Songs.Remove(song);
            }

            public Song GetSong(Guid artistId, Guid songId)
            {
                if (artistId == Guid.Empty)
                {
                    throw new ArgumentNullException(nameof(artistId));
                }

                if (songId == Guid.Empty)
                {
                    throw new ArgumentNullException(nameof(songId));
                }

                return _context.Songs
                  .Where(c => c.ArtistId == artistId && c.Id == songId).FirstOrDefault();
            }

            public async Task<Song> GetSongAsync(Guid artistId, Guid songId)
            {
                if (artistId == Guid.Empty)
                {
                    throw new ArgumentNullException(nameof(artistId));
                }

                if (songId == Guid.Empty)
                {
                    throw new ArgumentNullException(nameof(songId));
                }

                return await _context.Songs
                  .Where(c => c.ArtistId == artistId && c.Id == songId).FirstOrDefaultAsync();
            }

            public IEnumerable<Song> GetSongs(Guid artistId)
            {
                if (artistId == Guid.Empty)
                {
                    throw new ArgumentNullException(nameof(artistId));
                }

                return _context.Songs
                            .Where(c => c.ArtistId == artistId)
                            .OrderBy(c => c.Title).ToList();
            }


            public async Task<IEnumerable<Song>> GetSongsAsync(Guid artistId)
            {
                if (artistId == Guid.Empty)
                {
                    throw new ArgumentNullException(nameof(artistId));
                }

                return await _context.Songs
                            .Where(c => c.ArtistId == artistId)
                            .Include(m => m.Artist)
                            .OrderBy(c => c.Title).ToListAsync();
            }

            public void UpdateSong(Song song)
            {
                // no code in this implementation; stubbed only for Repository contract
            }
        #endregion

        #region "Artists"
        public void AddArtist(Artist artist)
        {
            if (artist == null)
            {
                throw new ArgumentNullException(nameof(artist));
            }

            // the repository fills the id (instead of using identity columns)
            artist.Id = Guid.NewGuid();

            foreach (var song in artist.Songs)
            {
                song.Id = Guid.NewGuid();
            }

            _context.Artists.Add(artist);
        }


        public bool ArtistExists(Guid artistId)
            {
                if (artistId == Guid.Empty)
                {
                    throw new ArgumentNullException(nameof(artistId));
                }

                return _context.Artists.Any(a => a.Id == artistId);
            }

            public void DeleteArtist(Artist artist)
            {
                if (artist == null)
                {
                    throw new ArgumentNullException(nameof(artist));
                }

                _context.Artists.Remove(artist);
            }

            public Artist GetArtist(Guid artistId)
            {
                if (artistId == Guid.Empty)
                {
                    throw new ArgumentNullException(nameof(artistId));
                }

                return _context.Artists.FirstOrDefault(a => a.Id == artistId);
            }

            public async Task<IEnumerable<Artist>> GetArtistsAsync()
            {
                return await _context.Artists.ToListAsync<Artist>();
            }

            public async Task<IEnumerable<Artist>> GetArtistsAsync(ArtistsResourceParameters artistsResourceParameters)
            {
                if (artistsResourceParameters == null)
                {
                    throw new ArgumentNullException(nameof(artistsResourceParameters));
                }

               
                var collection = _context.Artists as IQueryable<Artist>;

                if (!string.IsNullOrWhiteSpace(artistsResourceParameters.SearchQuery))
                {

                    var searchQuery = artistsResourceParameters.SearchQuery.Trim();
                    collection = collection.Where(a => a.FirstName.Contains(searchQuery)
                        || a.LastName.Contains(searchQuery));
                }

                return await collection.ToListAsync();
            }

            public async Task<IEnumerable<Artist>> GetArtistsAsync(IEnumerable<Guid> artistIds)
            {
                if (artistIds == null)
                {
                    throw new ArgumentNullException(nameof(artistIds));
                }

                return await _context.Artists.Where(a => artistIds.Contains(a.Id))
                    .OrderBy(a => a.FirstName)
                    .OrderBy(a => a.LastName)
                    .ToListAsync();
            }

            public void UpdateArtist(Artist artist)
            {
                // no code in this implementation
            }
        #endregion


        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }

        public async Task<bool> SaveChangesAsync()
        {
            //returns true if 1 or more entities were changed
            return (await _context.SaveChangesAsync() >= 0);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
               // dispose resources when needed
            }
        }

        
    }
}
