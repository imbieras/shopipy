using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using AppointmentManagement.DTOs;
using AppointmentManagement.Services;
using Shopipy.Persistence.Models;


namespace AppointmentManagement.Controllers;

[ApiController]
[Route("[controller]")]
public class AppointmentController : ControllerBase
{
    private readonly AppointmentService _appointmentService;
    private readonly IMapper _mapper;

    public AppointmentController(AppointmentService appointmentService, IMapper mapper)
    {
        _appointmentService = appointmentService;
        _mapper = mapper;
    }
    
    [HttpGet("{businessId}/employees/available")]
    public async Task<IActionResult> GetAvailableEmployees(int businessId, [FromQuery] int serviceId, [FromQuery] DateTime time)
    {
        try
        {
            var availableEmployees = await _appointmentService.GetAvailableEmployees(businessId, time, serviceId);
            
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
    
    [HttpGet("{businessId}/employees/{employeeId}/appointments")]
    public async Task<IActionResult> GetAppointmentsOfEmployee(
        int businessId,
        Guid employeeId,
        [FromQuery] DateTime time,
        [FromQuery] bool week = false)
    {
        var appointments = await _appointmentService.GetAppointmentsOfEmployee(businessId, employeeId, time, week);

        var serviceIds = appointments.Select(a => a.ServiceId).Distinct();
        var services = await _appointmentService.GetServicesByIdsAsync(serviceIds);
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
    
    [HttpGet("{businessId}/employees/{employeeId}/slots/{serviceId}")]
    public async Task<IActionResult> GetAvailableTimeSlots(
        int businessId, 
        Guid employeeId, 
        [FromQuery] DateTime date, 
        int serviceId)
    {
        try
        {
            var availableTimeSlots = await _appointmentService.GetAvailableTimeSlots(businessId, employeeId, date, serviceId);
    
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
    

    [HttpGet("{businessId}/appointments")]
    public async Task<IActionResult> GetAppointments(int businessId)
    {
        var appointments = await _appointmentService.GetAllAppointmentsInBusinessAsync(businessId);
        var responseDtos = _mapper.Map<IEnumerable<AppointmentResponseDto>>(appointments);

        return Ok(responseDtos);
    }

    [HttpGet("{businessId}/appointments/{id}")]
    public async Task<IActionResult> GetAppointment(int businessId, int id)
    {
        var appointment = await _appointmentService.GetAppointmentByIdInBusinessAsync(businessId, id);
        if (appointment == null) return NotFound();

        var responseDto = _mapper.Map<AppointmentResponseDto>(appointment);

        return Ok(responseDto);
    }

    [HttpPost("{businessId}/appointments")]
    public async Task<IActionResult> CreateAppointment(int businessId, AppointmentRequestDto request)
    {
        var appointment = _mapper.Map<Appointment>(request);
        appointment.BusinessId = businessId; // Associate with the business

        var createdAppointment = await _appointmentService.CreateAppointmentAsync(appointment);

        var responseDto = _mapper.Map<AppointmentResponseDto>(createdAppointment);

        return CreatedAtAction(nameof(GetAppointment), 
            new { businessId = businessId, id = createdAppointment.AppointmentId }, 
            responseDto);
    }

    [HttpPut("{businessId}/appointments/{id}")]
    public async Task<IActionResult> UpdateAppointment(int businessId, int id, AppointmentRequestDto request)
    {
        var existingAppointment = await _appointmentService.GetAppointmentByIdInBusinessAsync(businessId, id);
        if (existingAppointment == null) return NotFound();

        _mapper.Map(request, existingAppointment);
        var updatedAppointment = await _appointmentService.UpdateAppointmentAsync(existingAppointment);

        var responseDto = _mapper.Map<AppointmentResponseDto>(updatedAppointment);

        return Ok(responseDto);
    }

    [HttpDelete("{businessId}/appointments/{id}")]
    public async Task<IActionResult> DeleteAppointment(int businessId, int id)
    {
        var existingAppointment = await _appointmentService.GetAppointmentByIdInBusinessAsync(businessId, id);
        if (existingAppointment == null) return NotFound();

        var success = await _appointmentService.DeleteAppointmentAsync(id);
        if (!success) return BadRequest("Failed to delete appointment.");

        return NoContent();
    }
}
