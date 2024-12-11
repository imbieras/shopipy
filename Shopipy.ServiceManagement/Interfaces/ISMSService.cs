using Shopipy.Persistence.Models;

namespace Shopipy.ServiceManagement.Interfaces;

public interface ISMSService
{
    Task SendSMSAsync(string phoneNumber, string message);
}