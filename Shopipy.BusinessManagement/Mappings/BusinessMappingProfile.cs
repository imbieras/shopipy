using AutoMapper;
using Shopipy.BusinessManagement.DTOs;
using Shopipy.Persistence.Models;

namespace Shopipy.BusinessManagement.Mappings;

public class BusinessMappingProfile : Profile
{
    public BusinessMappingProfile()
    {
        // Map Business -> BusinessResponseDto
        CreateMap<Business, BusinessResponseDto>()
            .ForMember(dest => dest.BusinessId, opt => opt.MapFrom(src => src.BusinessId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.VATNumber, opt => opt.MapFrom(src => src.VATNumber))
            .ForMember(dest => dest.BusinessType, opt => opt.MapFrom(src => src.BusinessType))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

        // Map BusinessRequestDto -> Business
        CreateMap<BusinessRequestDto, Business>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.BusinessName))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.BusinessAddress))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.BusinessPhone))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.BusinessEmail))
            .ForMember(dest => dest.VATNumber, opt => opt.MapFrom(src => src.BusinessVatNumber))
            .ForMember(dest => dest.BusinessType, opt => opt.MapFrom(src => src.BusinessType))
            .ForMember(dest => dest.BusinessId, opt => opt.Ignore()) 
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) 
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
    }
}