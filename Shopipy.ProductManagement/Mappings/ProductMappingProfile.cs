using AutoMapper;
using Shopipy.Persistence.Models;
using Shopipy.ProductManagement.DTOs;

namespace Shopipy.ProductManagement.Mappings
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<Product, ProductResponseDTO>();

            CreateMap<ProductRequestDTO, Product>()
                .ForMember(dest => dest.BusinessId, opt => opt.Ignore()) 
                .ForMember(dest => dest.ProductId, opt => opt.Ignore()) 
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) 
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.BasePrice, opt => opt.MapFrom(src => src.BasePrice))
                .ForMember(dest => dest.ProductState, opt => opt.MapFrom(src => src.ProductState));
        }

    }
}
