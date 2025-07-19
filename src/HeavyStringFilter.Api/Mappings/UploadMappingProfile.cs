using AutoMapper;
using HeavyStringFilter.Api.Models;
using HeavyStringFilter.Application.Models;

namespace HeavyStringFilter.Api.Mappings;

public class UploadMappingProfile : Profile
{
    public UploadMappingProfile()
    {
        CreateMap<UploadChunkRequest, UploadChunkDto>();
    }
}
