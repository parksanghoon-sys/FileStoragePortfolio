using AutoMapper;
using FileStorage.IdentityService.Application.DTOs;
using FileStorage.IdentityService.Domain;

namespace FileStorage.IdentityService.Application.MappingProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>();
        }
    }
}