using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Shopipy.Persistence.Models;
using Shopipy.ServiceManagement.DTOs;
using Shopipy.Shared.Services;

namespace Shopipy.ServiceManagement.Services;

[Authorize]
[ApiController]
[EnableRateLimiting("fixed")]
[Route("businesses/{businessId}/appointments")]
public class AppointmentController(IAppointmentService appointmentService, IMapper mapper) : ControllerBase
{
    
    [HttpGet("available-employees")]
    public async Task<IActionResult> GetAvailableEmployees(int businessId, [FromQuery] int serviceId, [FromQuery] DateTime time)
    {
        try
        {
            var availableEmployees = await appointmentService.GetAvailableEmployees(businessId, time, serviceId);
            
            if (!availableEmployees.Any())
            {
                return NotFound(new { message = $"No available employees for this service at the given time: {availableEmployees}." });
            }

            var responseDto = availableEmployees.Select(a => new
            {
                employee_id = a.Id,
                employee_name = a.Name
            });
            
            return Ok(responseDto);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    [HttpGet("employees/{employeeId}")]
    public async Task<IActionResult> GetAppointmentsOfEmployee(
        int businessId,
        Guid employeeId,
        [FromQuery] DateTime time,
        [FromQuery] bool week = false)
    {
        var appointments = await appointmentService.GetAppointmentsOfEmployee(businessId, employeeId, time, week);

        var serviceIds = appointments.Select(a => a.ServiceId).Distinct();
        var services = await appointmentService.GetServicesByIdsAsync(serviceIds);
        var serviceNameLookup = services.ToDictionary(s => s.ServiceId, s => s.ServiceName);
        
        var result = appointments.Select(a => new
        {
            appointment_id = a.AppointmentId,
            customer_name = a.CustomerName,
            service_id = a.ServiceId,
            service_name = serviceNameLookup.TryGetValue(a.ServiceId, out var name) ? name : "Unknown",
            start_time = a.StartTime,
            end_time = a.EndTime
        });

        return Ok(result);
    }
    
    [HttpGet("employees/{employeeId}/services/{serviceId}/slots")]
    public async Task<IActionResult> GetAvailableTimeSlots(
        int businessId, 
        Guid employeeId, 
        [FromQuery] DateTime date, 
        int serviceId)
    {
        try
        {
            var availableTimeSlots = await appointmentService.GetAvailableTimeSlots(businessId, employeeId, date, serviceId);
    
            if (!availableTimeSlots.Any())
            {
                return NotFound(new { message = "No available time slots for the given employee on this date." });
            }
    
            var response = availableTimeSlots.Select(slot => new
            {
                available_time = slot.ToString("o") 
            });
    
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    

    [HttpGet]
    public async Task<IActionResult> GetAppointments(int businessId)
    {
        var appointments = await appointmentService.GetAllAppointmentsInBusinessAsync(businessId);
        var responseDtos = mapper.Map<IEnumerable<AppointmentResponseDto>>(appointments);

        return Ok(responseDtos);
    }

    [HttpGet("{appointmentId}")]
    public async Task<IActionResult> GetAppointment(int businessId, int appointmentId)
    {
        var appointment = await appointmentService.GetAppointmentByIdInBusinessAsync(businessId, appointmentId);
        if (appointment == null) return NotFound();

        var responseDto = mapper.Map<AppointmentResponseDto>(appointment);

        return Ok(responseDto);
    }
    
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> CreateAppointment(int businessId, AppointmentRequestDto request)
    {
        var appointment = mapper.Map<Appointment>(request);
        appointment.BusinessId = businessId;

        try
        {
            var createdAppointment = await appointmentService.CreateAppointmentAsync(appointment, request.SendSmsNotification);
            var responseDto = mapper.Map<AppointmentResponseDto>(createdAppointment);

            return CreatedAtAction(
                nameof(GetAppointment), 
                new { businessId = businessId, appointmentId = createdAppointment.AppointmentId },
                responseDto
            );
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("past"))
        {
            return BadRequest("Cannot create appointments in the past.");
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("already booked"))
        {
            return BadRequest("This time slot is already booked. Please choose another time.");
        }
    }

    [AllowAnonymous]
    [HttpPut("{appointmentId}")]
    public async Task<IActionResult> UpdateAppointment(int businessId, int appointmentId, AppointmentRequestDto request)
    {
        var existingAppointment = await appointmentService.GetAppointmentByIdInBusinessAsync(businessId, appointmentId);
        if (existingAppointment == null) return NotFound();

        mapper.Map(request, existingAppointment);
        var updatedAppointment = await appointmentService.UpdateAppointmentAsync(existingAppointment, request.SendSmsNotification);

        var responseDto = mapper.Map<AppointmentResponseDto>(updatedAppointment);

        return Ok(responseDto);
    }

    [AllowAnonymous]
    [HttpDelete("{appointmentId}")]
    public async Task<IActionResult> DeleteAppointment(int businessId, int appointmentId, [FromQuery] bool smsNotification = false)
    {
        var existingAppointment = await appointmentService.GetAppointmentByIdInBusinessAsync(businessId, appointmentId);
        if (existingAppointment == null) return NotFound();

        var success = await appointmentService.DeleteAppointmentAsync(appointmentId, smsNotification);
        if (!success) return BadRequest("Failed to delete appointment.");

        return NoContent();
    }
}
