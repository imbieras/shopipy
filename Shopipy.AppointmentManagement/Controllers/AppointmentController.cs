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
