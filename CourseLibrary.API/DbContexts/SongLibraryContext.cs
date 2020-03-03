using SongLibrary.API.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace SongLibrary.API.DbContexts
{
    public class SongLibraryContext : DbContext
    {
        public SongLibraryContext(DbContextOptions<SongLibraryContext> options)
           : base(options)
        {
        }

        public DbSet<Artist> Artists { get; set; }
        public DbSet<Song> Songs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // seed the database with dummy data
            modelBuilder.Entity<Artist>().HasData(
                new Artist()
                {
                    Id = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
                    FirstName = "Elvis",
                    LastName = "Presley",
                },
                new Artist()
                {
                    Id = Guid.Parse("da2fd609-d754-4feb-8acd-c4f9ff13ba96"),
                    FirstName = "Bob",
                    LastName = "Dylan",
                },
                new Artist()
                {
                    Id = Guid.Parse("2902b665-1190-4c70-9915-b9c2d7680450"),
                    FirstName = "Katy",
                    LastName = "Perry",
                },
                new Artist()
                {
                    Id = Guid.Parse("102b566b-ba1f-404c-b2df-e2cde39ade09"),
                    FirstName = "Snoop",
                    LastName = "Dogg",
                },
                new Artist()
                {
                    Id = Guid.Parse("5b3621c0-7b12-4e80-9c8b-3398cba7ee05"),
                    FirstName = "Bruno",
                    LastName = "Mars",
                },
                new Artist()
                {
                    Id = Guid.Parse("2aadd2df-7caf-45ab-9355-7f6332985a87"),
                    FirstName = "Johnny",
                    LastName = "Cash",
                },
                new Artist()
                {
                    Id = Guid.Parse("2ee49fe3-edf2-4f91-8409-3eb25ce6ca51"),
                    FirstName = "Jay",
                    LastName = "Z",
                }
                );

            modelBuilder.Entity<Song>().HasData(
               new Song
               {
                   Id = Guid.Parse("5b1c2b4d-48c7-402a-80c3-cc796ad49c6b"),
                   ArtistId = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
                   Title = "Hound Dog",
                   Genre = "Rock and Roll",
                   Filename = "",
               },
               new Song
               {
                   Id = Guid.Parse("d8663e5e-7494-4f81-8739-6e0de1bea7ee"),
                   ArtistId = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
                   Title = "Viva Las Vegas",
                   Genre = "Rock and Roll",
                   Filename = "",
               },
               new Song
               {
                   Id = Guid.Parse("d173e20d-159e-4127-9ce9-b0ac2564ad97"),
                   ArtistId = Guid.Parse("da2fd609-d754-4feb-8acd-c4f9ff13ba96"),
                   Title = "Knocking on Heaven's Door",
                   Genre = "Rock and Roll",
                   Filename = "",
               },
               new Song
               {
                   Id = Guid.Parse("40ff5488-fdab-45b5-bc3a-14302d59869a"),
                   ArtistId = Guid.Parse("2902b665-1190-4c70-9915-b9c2d7680450"),
                   Title = "Roar",
                   Genre = "Pop",
                   Filename = "",
               }
               );

            base.OnModelCreating(modelBuilder);
        }
    }
}
