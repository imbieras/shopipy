using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Shopipy.Persistence.Models;
using Shopipy.ServiceManagement.DTOs;
using Shopipy.Shared;
using Shopipy.Shared.Services;

namespace Shopipy.ServiceManagement.Controllers;

[ApiController]
[EnableRateLimiting("fixed")]
[Route("businesses/{businessId:int}/appointments")]
[Authorize(Policy = AuthorizationPolicies.RequireBusinessAccess)]
public class AppointmentController(IAppointmentService appointmentService, IBusinessService businessService, IMapper mapper, ILogger<AppointmentController> logger) : ControllerBase
{

    [HttpGet("available-employees")]
    public async Task<IActionResult> GetAvailableEmployees(int businessId, [FromQuery] int serviceId, [FromQuery] DateTime time)
    {
        try
        {
            var availableEmployees = await appointmentService.GetAvailableEmployees(businessId, time, serviceId);

            if (availableEmployees.Count == 0)
            {
                logger.LogWarning("No available employees for Service ID {ServiceId} at {Time}.", serviceId, time);
                return NotFound(new { message = "No available employees for this service at the given time." });
            }

            var responseDto = availableEmployees.Select(a => new { employee_id = a.Id, employee_name = a.Name });

            return Ok(responseDto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching available employees for Service ID {ServiceId}.", serviceId);
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("employees/{employeeId:guid}")]
    public async Task<IActionResult> GetAppointmentsOfEmployee(
        int businessId,
        Guid employeeId,
        [FromQuery] DateTime time,
        [FromQuery] bool week = false
    )
    {
        var appointments = await appointmentService.GetAppointmentsOfEmployee(businessId, employeeId, time, week);

        var appointmentList = appointments.ToList();
        var serviceIds = appointmentList.Select(a => a.ServiceId).Distinct();
        var services = await appointmentService.GetServicesByIdsAsync(serviceIds);
        var serviceNameLookup = services.ToDictionary(s => s.ServiceId, s => s.ServiceName);

        var result = appointmentList.Select(a => new
        {
            appointment_id = a.AppointmentId,
            customer_name = a.CustomerName,
            service_id = a.ServiceId,
            service_name = serviceNameLookup.GetValueOrDefault(a.ServiceId, "Unknown"),
            start_time = a.StartTime,
            end_time = a.EndTime
        });

        return Ok(result);
    }

    [HttpGet("employees/{employeeId:guid}/services/{serviceId:int}/slots")]
    public async Task<IActionResult> GetAvailableTimeSlots(
        int businessId,
        Guid employeeId,
        [FromQuery] DateTime date,
        int serviceId
    )
    {
        try
        {
            var availableTimeSlots = await appointmentService.GetAvailableTimeSlots(businessId, employeeId, date, serviceId);

            if (availableTimeSlots.Count == 0)
            {
                logger.LogWarning("No available time slots for Employee ID {EmployeeId} on {Date}.", employeeId, date);
                return NotFound(new { message = "No available time slots for the given employee on this date." });
            }

            var response = availableTimeSlots.Select(slot => new { available_time = slot.ToString("o") });

            return Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching time slots for Employee ID {EmployeeId}.", employeeId);
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

    [HttpGet("{appointmentId:int}")]
    public async Task<IActionResult> GetAppointment(int businessId, int appointmentId)
    {
        var appointment = await appointmentService.GetAppointmentByIdInBusinessAsync(businessId, appointmentId);
        if (appointment == null)
        {
            logger.LogWarning("Appointment ID {AppointmentId} not found.", appointmentId);
            return NotFound();
        }

        var responseDto = mapper.Map<AppointmentResponseDto>(appointment);

        return Ok(responseDto);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAppointment(int businessId, AppointmentRequestDto request)
    {
        var business = await businessService.GetBusinessByIdAsync(businessId);
        if (business == null)
        {
            logger.LogWarning("Business with ID {BusinessId} not found for appointment creation.", businessId);
            return NotFound();
        }

        var appointment = mapper.Map<Appointment>(request);
        appointment.BusinessId = businessId;

        try
        {
            var createdAppointment = await appointmentService.CreateAppointmentAsync(appointment, request.SendSmsNotification);
            var responseDto = mapper.Map<AppointmentResponseDto>(createdAppointment);

            return CreatedAtAction(
            nameof(GetAppointment),
            new { businessId, appointmentId = createdAppointment.AppointmentId },
            responseDto
            );
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("past"))
        {
            logger.LogError(ex, "Cannot create appointments in the past.");
            return BadRequest("Cannot create appointments in the past.");
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("already booked"))
        {
            logger.LogError(ex, "This time slot is already booked. Please choose another time.");
            return BadRequest("This time slot is already booked. Please choose another time.");
        }
    }

    [HttpPut("{appointmentId:int}")]
    public async Task<IActionResult> UpdateAppointment(int businessId, int appointmentId, AppointmentRequestDto request)
    {
        var existingAppointment = await appointmentService.GetAppointmentByIdInBusinessAsync(businessId, appointmentId);
        if (existingAppointment == null)
        {
            logger.LogWarning("Appointment ID {AppointmentId} not found.", appointmentId);
            return NotFound();
        }

        mapper.Map(request, existingAppointment);
        var updatedAppointment = await appointmentService.UpdateAppointmentAsync(existingAppointment, request.SendSmsNotification);

        var responseDto = mapper.Map<AppointmentResponseDto>(updatedAppointment);

        return Ok(responseDto);
    }

    [HttpDelete("{appointmentId:int}")]
    public async Task<IActionResult> DeleteAppointment(int businessId, int appointmentId, [FromQuery] bool smsNotification = false)
    {
        var existingAppointment = await appointmentService.GetAppointmentByIdInBusinessAsync(businessId, appointmentId);
        if (existingAppointment == null)
        {
            logger.LogWarning("Appointment ID {AppointmentId} not found.", appointmentId);
            return NotFound();
        }

        var success = await appointmentService.DeleteAppointmentAsync(appointmentId, smsNotification);
        if (success)
        {
            return NoContent();
        }

        logger.LogError("Failed to delete Appointment ID {AppointmentId}.", appointmentId);
        return NotFound();
    }
}