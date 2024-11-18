using AutoMapper;
using Shopipy.Persistence.Models;
using Shopipy.ServiceManagement.DTOs;

namespace Shopipy.ServiceManagement.Mappings;

public class ServiceMappingProfile : Profile
{
    public ServiceMappingProfile()
    {
        // Map Service -> ServiceResponseDto
        CreateMap<Service, ServiceResponseDto>()
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
            .ForMember(dest => dest.ServiceId, opt => opt.MapFrom(src => src.ServiceId))
            .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.ServiceName))
            .ForMember(dest => dest.ServiceDescription, opt => opt.MapFrom(src => src.ServiceDescription))
            .ForMember(dest => dest.ServiceBasePrice, opt => opt.MapFrom(src => src.ServiceBasePrice))
            .ForMember(dest => dest.ServiceDuration, opt => opt.MapFrom(src => src.ServiceDuration))
            .ForMember(dest => dest.ServiceServiceCharge, opt => opt.MapFrom(src => src.ServiceServiceCharge))
            .ForMember(dest => dest.IsServiceActive, opt => opt.MapFrom(src => src.IsServiceActive))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

        // Map ServiceRequestDto -> Service
        CreateMap<ServiceRequestDto, Service>()
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
            .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.ServiceName))
            .ForMember(dest => dest.ServiceDescription, opt => opt.MapFrom(src => src.ServiceDescription))
            .ForMember(dest => dest.ServiceBasePrice, opt => opt.MapFrom(src => src.ServiceBasePrice))
            .ForMember(dest => dest.ServiceDuration, opt => opt.MapFrom(src => src.ServiceDuration))
            .ForMember(dest => dest.ServiceServiceCharge, opt => opt.MapFrom(src => src.ServiceServiceCharge))
            .ForMember(dest => dest.IsServiceActive, opt => opt.MapFrom(src => src.IsServiceActive))
            .ForMember(dest => dest.ServiceId, opt => opt.Ignore()) 
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) 
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore()); 
    }
}
