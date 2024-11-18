using AutoMapper;
using Shopipy.CategoryManagement.DTOs;
using Shopipy.Persistence.Models;

namespace Shopipy.CategoryManagement.Mappings
{
    public class CategoryMappingProfile : Profile
    {
        public CategoryMappingProfile()
        {
            // Map Category -> CategoryResponseDto
            CreateMap<Category, CategoryResponseDto>()
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

            // Map CategoryRequestDto -> Category
            CreateMap<CategoryRequestDto, Category>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.CategoryId, opt => opt.Ignore());
        }
    }
}
