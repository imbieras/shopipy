using AutoMapper;
using Shopipy.Persistence.Models;
using Shopipy.UserManagement.Dtos;

namespace Shopipy.UserManagement.Mappings;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<UserBaseDto, User>()
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phone))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.BusinessId, opt => opt.MapFrom(src => src.BusinessId))
            .ForMember(dest => dest.Business, opt => opt.Ignore()) 
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.NormalizedUserName, opt => opt.Ignore())
            .ForMember(dest => dest.NormalizedEmail, opt => opt.Ignore())
            .ForMember(dest => dest.EmailConfirmed, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.SecurityStamp, opt => opt.Ignore())
            .ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore())
            .ForMember(dest => dest.PhoneNumberConfirmed, opt => opt.Ignore())
            .ForMember(dest => dest.TwoFactorEnabled, opt => opt.Ignore())
            .ForMember(dest => dest.LockoutEnd, opt => opt.Ignore())
            .ForMember(dest => dest.LockoutEnabled, opt => opt.Ignore())
            .ForMember(dest => dest.AccessFailedCount, opt => opt.Ignore())
            .ForMember(dest => dest.UserState, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        CreateMap<UserUpdateRequestDto, User>()
            .IncludeBase<UserBaseDto, User>()
            .ForMember(dest => dest.UserState, opt => opt.MapFrom(src => src.UserState))
            .ForMember(dest => dest.BusinessId, opt => opt.MapFrom(src => src.BusinessId))
            .ForMember(dest => dest.Business, opt => opt.Ignore());
        
        CreateMap<User, UserBaseDto>()
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.BusinessId, opt => opt.MapFrom(src => src.BusinessId));

        CreateMap<User, UserResponseDto>()
            .IncludeBase<User, UserBaseDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id));
    }
}