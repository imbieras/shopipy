using AutoMapper;
using Shopipy.GiftCardManagement.DTOs;
using Shopipy.Persistence.Models;

namespace Shopipy.GiftcardManagement.Mappings;

public class GiftCardMappingProfile : Profile
{
    public GiftCardMappingProfile()
    {
        CreateMap<GiftCard, GiftCardResponseDTO>();

        CreateMap<GiftCardRequestDTO, GiftCard>()
            .ForMember(dest => dest.BusinessId, opt => opt.Ignore()) 
            .ForMember(dest => dest.GiftCardId, opt => opt.Ignore()) 
            .ForMember(dest => dest.AmountLeft, opt => opt.MapFrom(src => src.AmountOriginal)) 
            .ForMember(dest => dest.GiftCardCode, opt => opt.Ignore()) 
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) 
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore()); 
    }
}
