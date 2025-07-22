using AutoMapper;
using FileStorage.FileService.Application.DTOs;
using FileStorage.FileService.Domain;

namespace FileStorage.FileService.Application.MappingProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<FileEntity, FileDto>();
            CreateMap<Comment, CommentDto>(); // Username은 서비스에서 채워질 예정
            CreateMap<FileShare, SharedFileDto>(); // SharedBy는 서비스에서 채워질 예정
        }
    }
}