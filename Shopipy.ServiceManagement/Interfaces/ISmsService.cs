namespace Shopipy.ServiceManagement.Interfaces;

public interface ISmsService
{
    Task SendSmsAsync(string phoneNumber, string message);
}