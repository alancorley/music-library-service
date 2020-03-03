using SongLibrary.API.Entities;
using SongLibrary.API.ResourceParameters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SongLibrary.API.Services
{
    public interface ISongLibraryRepository
    {
        #region "Songs"
            Task<IEnumerable<Song>> GetSongsAsync(Guid artistId);
            Task<Song> GetSongAsync(Guid artistId, Guid songId);

            Task AddSongAsync(Guid artistId, Song song);
            void UpdateSong(Song song);
            void DeleteSong(Song song);
        #endregion

        #region "Artists"
            Task<IEnumerable<Artist>> GetArtistsAsync();
            Task<IEnumerable<Artist>> GetArtistsAsync(ArtistsResourceParameters artistsResourceParameters);
            Artist GetArtist(Guid artistId);
            Task<IEnumerable<Artist>> GetArtistsAsync(IEnumerable<Guid> artistIds);
            void AddArtist(Artist artist);
            void DeleteArtist(Artist artist);
            void UpdateArtist(Artist artist);
            bool ArtistExists(Guid artistId);
        #endregion

        
        bool Save();

        Task<bool> SaveChangesAsync();
    }
}
