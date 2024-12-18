using AutoMapper;
using Shopipy.Persistence.Models;
using Shopipy.ProductManagement.DTOs;

namespace Shopipy.ProductManagement.Mappings;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<Product, ProductResponseDto>();

        CreateMap<ProductRequestDto, Product>()
            .ForMember(dest => dest.BusinessId, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        CreateMap<ProductVariation, ProductVariationResponseDto>();

        CreateMap<ProductVariationRequestDto, ProductVariation>()
            .ForMember(dest => dest.VariationId, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
    }

}