using AutoMapper;
using Shopipy.CategoryManagement.DTOs;
using Shopipy.Persistence.Models;

namespace Shopipy.CategoryManagement.Mappings;

public class CategoryMappingProfile : Profile
{
    public CategoryMappingProfile()
    {
        CreateMap<Category, CategoryResponseDto>();

        CreateMap<CategoryRequestDto, Category>()
            .ForMember(dest => dest.BusinessId, opt => opt.Ignore())
            .ForMember(dest => dest.CategoryId, opt => opt.Ignore());
    }
}