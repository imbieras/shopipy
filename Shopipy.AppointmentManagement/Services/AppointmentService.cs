using Shopipy.Persistence.Models;
using Shopipy.Persistence.Repositories;

namespace AppointmentManagement.Services;

public class AppointmentService
{
    private readonly IGenericRepository<Appointment> _appointmentRepository;

    public AppointmentService(IGenericRepository<Appointment> appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<IEnumerable<Appointment>> GetAllAppointments()
    {
        return await _appointmentRepository.GetAllAsync();
    }

    public async Task<Appointment> GetAppointmentById(int id)
    {
        return await _appointmentRepository.GetByIdAsync(id);
    }

    public async Task<Appointment> CreateAppointment(Appointment appointment)
    {
        return await _appointmentRepository.AddAsync(appointment);
    }

    public async Task<Appointment> UpdateAppointment(Appointment appointment)
    {
        return await _appointmentRepository.UpdateAsync(appointment);
    }

    public async Task<bool> DeleteAppointment(int id)
    {
        return await _appointmentRepository.DeleteAsync(id);
    }
}