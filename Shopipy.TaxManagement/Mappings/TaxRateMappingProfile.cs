using AutoMapper;
using Shopipy.Persistence.Models;
using Shopipy.TaxManagement.DTOs;

namespace Shopipy.TaxManagement.Mappings;

public class TaxRateMappingProfile : Profile
{
    public TaxRateMappingProfile()
    {
        CreateMap<TaxRate, TaxRateResponseDto>();
        CreateMap<TaxRateRequestDto, TaxRate>()
            .ForMember(dest => dest.TaxRateId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.BusinessId, opt => opt.Ignore());
    }
}