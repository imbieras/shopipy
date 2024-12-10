using Shopipy.Persistence.Models;
using Shopipy.ServiceManagement.Interfaces;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.TwiML.Messaging;

namespace Shopipy.ServiceManagement.Services;

public class TwilioSMSService : ISMSService
{
    private readonly string _accountSid;
    private readonly string _authToken;
    private readonly string _fromNumber;

    public TwilioSMSService(string accountSid, string authToken, string fromNumber)
    {
        _accountSid = accountSid;
        _authToken = authToken;
        _fromNumber = fromNumber;

        TwilioClient.Init(_accountSid, _authToken);
    }

    public async Task SendSMSAsync(string phoneNumber, string message)
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