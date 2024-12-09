using AutoMapper;
using Shopipy.DiscountManagement.DTOs;
using Shopipy.Persistence.Models;

namespace Shopipy.DiscountManagement.Mappings;

public class DiscountMappingProfile : Profile
{
    public DiscountMappingProfile()
    {
        CreateMap<Discount, DiscountResponseDto>();

        CreateMap<DiscountRequestDto, Discount>()
            .ForMember(d => d.DiscountId, opt => opt.Ignore())
            .ForMember(d => d.BusinessId, opt => opt.Ignore())
            .ForMember(d => d.CreatedAt, opt => opt.Ignore())
            .ForMember(d => d.UpdatedAt, opt => opt.Ignore());
    }
}