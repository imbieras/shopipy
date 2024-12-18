using Shopipy.ServiceManagement.Interfaces;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Shopipy.ServiceManagement.Services;

public class TwilioSmsService : ISmsService
{
    private readonly string _fromNumber;

    public TwilioSmsService(string accountSid, string authToken, string fromNumber)
    {
        _fromNumber = fromNumber;

        TwilioClient.Init(accountSid, authToken);
    }

    public async Task SendSmsAsync(string phoneNumber, string message)
    {
        try
        {
            await MessageResource.CreateAsync(
            body: message,
            from: new Twilio.Types.PhoneNumber(_fromNumber),
            to: new Twilio.Types.PhoneNumber(phoneNumber)
            );
        }
        catch (Twilio.Exceptions.ApiException apiEx)
        {
            Console.WriteLine($"Twilio API Error: {apiEx.Message}, Code: {apiEx.Code}");
            throw;
        }
        catch (Twilio.Exceptions.TwilioException twilioEx)
        {
            Console.WriteLine($"Twilio Error: {twilioEx.Message}");
            throw;
        }
        catch (ArgumentException argEx)
        {
            Console.WriteLine($"Invalid phone number format: {argEx.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error sending SMS: {ex.Message}");
            throw;
        }
    }
}