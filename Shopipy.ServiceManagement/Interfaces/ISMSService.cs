using Shopipy.Persistence.Models;

namespace Shopipy.ServiceManagement.Interfaces;

public interface ISMSService
{
    Task SendAppointmentConfirmationAsync(Appointment appointment, string serviceName);
}