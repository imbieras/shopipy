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

    [HttpGet]
    public async Task<IActionResult> GetAppointments()
    {
        var appointments = await _appointmentService.GetAllAppointments();
        var responseDtos = _mapper.Map<IEnumerable<AppointmentResponseDto>>(appointments);

        return Ok(responseDtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAppointment(int id)
    {
        var appointment = await _appointmentService.GetAppointmentById(id);
        if (appointment == null) return NotFound();

        var responseDto = _mapper.Map<AppointmentResponseDto>(appointment);

        return Ok(responseDto);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAppointment(AppointmentRequestDto request)
    {
        var appointment = _mapper.Map<Appointment>(request);

        var createdAppointment = await _appointmentService.CreateAppointment(appointment);

        var responseDto = _mapper.Map<AppointmentResponseDto>(createdAppointment);

        return CreatedAtAction(nameof(GetAppointment), new { id = createdAppointment.AppointmentId }, responseDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAppointment(int id, AppointmentRequestDto request)
    {
        var existingAppointment = await _appointmentService.GetAppointmentById(id);
        if (existingAppointment == null) return NotFound();

        _mapper.Map(request, existingAppointment);
        var updatedAppointment = await _appointmentService.UpdateAppointment(existingAppointment);

        var responseDto = _mapper.Map<AppointmentResponseDto>(updatedAppointment);

        return Ok(responseDto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAppointment(int id)
    {
        var success = await _appointmentService.DeleteAppointment(id);
        if (!success) return NotFound();

        return NoContent();
    }
}
