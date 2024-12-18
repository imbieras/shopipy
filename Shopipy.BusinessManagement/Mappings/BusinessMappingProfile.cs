using AutoMapper;
using Shopipy.BusinessManagement.DTOs;
using Shopipy.Persistence.Models;

namespace Shopipy.BusinessManagement.Mappings;

public class BusinessMappingProfile : Profile
{
    public BusinessMappingProfile()
    {
        CreateMap<Business, BusinessResponseDto>();

        CreateMap<BusinessRequestDto, Business>()
            .ForMember(dest => dest.BusinessId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
    }
}