using ApiKeysApi.DataAccess.Entities;
using ApiKeysApi.DTOs.Request;
using AutoMapper;

namespace ApiKeysApi.Mapper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<ApiKeyRequest, ApiKey>()
            .ForMember( x => x.CreatedAt, opt => opt.MapFrom(_ => DateTime.Now))
            .ForMember( x => x.ModifiedAt, opt => opt.MapFrom(_ => DateTime.Now));

        CreateMap<ApiKeyUpdateRequest, ApiKey>();
    }
}