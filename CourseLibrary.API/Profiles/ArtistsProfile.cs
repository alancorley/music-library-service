using AutoMapper;
using SongLibrary.API.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SongLibrary.API.Profiles
{
    public class ArtistsProfile : Profile
    {
        public ArtistsProfile()
        {
            //CreateMap<Entities.Artist, Models.ArtistDto>()
            //    .ForMember(
            //        dest => dest.Name,
            //        opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));
                    

            CreateMap<Entities.Artist, Models.ArtistDto>();
            CreateMap<Models.ArtistForUpdateDto, Entities.Artist>();
            CreateMap<Models.ArtistForCreationDto, Entities.Artist>();
        }
    }
}
