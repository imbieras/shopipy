using AutoMapper;
using Shopipy.Persistence.Models;
using Shopipy.ServiceManagement.DTOs;

namespace Shopipy.ServiceManagement.Mappings;

public class ServiceMappingProfile : Profile
{
    public ServiceMappingProfile()
    {
        // Map Service -> ServiceResponseDto
        CreateMap<Service, ServiceResponseDto>();

        // Map ServiceRequestDto -> Service
        CreateMap<ServiceRequestDto, Service>()
            .ForMember(dest => dest.BusinessId, opt => opt.Ignore()) 
            .ForMember(dest => dest.ServiceId, opt => opt.Ignore()) 
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) 
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore()); 
    }
}