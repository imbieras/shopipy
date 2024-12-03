using Shopipy.Persistence.Models;
using AutoMapper;
using Shopipy.ProductManagement.DTOs;

namespace Shopipy.ProductManagement.Mappings
{
    public class ProductVariationMappingProfile : Profile
    {
        public ProductVariationMappingProfile()
        {
            CreateMap<ProductVariation, ProductVariationResponseDTO>();

            CreateMap<ProductVariationRequestDTO, ProductVariation>()
                .ForMember(dest => dest.VariationId, opt => opt.Ignore()) 
                .ForMember(dest => dest.ProductId, opt => opt.Ignore()) 
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) 
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore()); 

        }
    }
}
