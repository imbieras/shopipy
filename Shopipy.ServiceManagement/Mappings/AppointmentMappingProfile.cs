using AutoMapper;
using Shopipy.Persistence.Models;
using Shopipy.ServiceManagement.DTOs;

namespace Shopipy.ServiceManagement.Mappings;

public class AppointmentMappingProfile : Profile
{
    public AppointmentMappingProfile()
    {
        CreateMap<Appointment, AppointmentResponseDto>();

        CreateMap<AppointmentRequestDto, Appointment>()
            .ForMember(dest => dest.BusinessId, opt => opt.Ignore())
            .ForMember(dest => dest.AppointmentId, opt => opt.Ignore())
            .ForMember(dest => dest.EndTime, opt => opt.Ignore());
    }
}