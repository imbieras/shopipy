using Shopipy.Persistence.Models;
using Shopipy.ServiceManagement.Interfaces;

namespace Shopipy.ServiceManagement.Services;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

public class TwilioSMSService: ISMSService
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
    
    public async Task SendAppointmentConfirmationAsync(Appointment appointment, string serviceName)
    {
        try
        {
            var message = $"Your appointment for {serviceName} has been confirmed for {appointment.StartTime:g}. " +
                          $"Thank you for booking with us!";

            await MessageResource.CreateAsync(
                body: message,
                from: new Twilio.Types.PhoneNumber(_fromNumber),
                to: new Twilio.Types.PhoneNumber(appointment.CustomerPhone)
            );
        }
        catch (Twilio.Exceptions.ApiException apiEx)
        {
            // invalid credentials, invalid etc.
            Console.WriteLine($"Twilio API Error: {apiEx.Message}, Code: {apiEx.Code}");
        }
        catch (Twilio.Exceptions.TwilioException twilioEx)
        {
            // general twilio errors
            Console.WriteLine($"Twilio Error: {twilioEx.Message}");
        }
        catch (ArgumentException argEx)
        {
            // invalid phone number
            Console.WriteLine($"Invalid phone number format: {argEx.Message}");
        }
        catch (Exception ex)
        {
            // unexpected error
            Console.WriteLine($"Unexpected error sending SMS: {ex.Message}");
        }
    }
    
}