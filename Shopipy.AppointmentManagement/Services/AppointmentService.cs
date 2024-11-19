using Shopipy.Persistence.Models;
using Shopipy.Persistence.Repositories;

namespace AppointmentManagement.Services;

public class AppointmentService
{
    private readonly IGenericRepository<Appointment> _appointmentRepository;
    private readonly IGenericRepository<Service> _serviceRepository;

    public AppointmentService(IGenericRepository<Appointment> appointmentRepository,
        IGenericRepository<Service> serviceRepository)
    {
        _appointmentRepository = appointmentRepository;
        _serviceRepository = serviceRepository;
    }

    public async Task<IEnumerable<Appointment>> GetAllAppointments()
    {
        return await _appointmentRepository.GetAllAsync();
    }

    public async Task<IEnumerable<Appointment>> GetAllAppointmentsInBusinessAsync(int businessId)
    {
        return await _appointmentRepository.GetAllByConditionAsync(a => a.BusinessId == businessId);
    }

    public async Task<Appointment> GetAppointmentById(int id)
    {
        return await _appointmentRepository.GetByIdAsync(id);
    }

    public async Task<Appointment> GetAppointmentByIdInBusinessAsync(int businessId, int id)
    {
        return await _appointmentRepository.GetByConditionAsync(a => a.BusinessId == businessId && a.AppointmentId == id);
    }

    public async Task<Appointment> CreateAppointmentAsync(Appointment appointment)
    {
        var service = await _serviceRepository.GetByIdAsync(appointment.ServiceId);
        
        if (service == null)
        {
            throw new ArgumentException("Service not found");
        }

        appointment.EndTime = appointment.StartTime.AddMinutes(service.ServiceDuration);

        return await _appointmentRepository.AddAsync(appointment);
    }

    public async Task<Appointment> UpdateAppointmentAsync(Appointment appointment)
    {
        return await _appointmentRepository.UpdateAsync(appointment);
    }

    public async Task<bool> DeleteAppointmentAsync(int id)
    {
        return await _appointmentRepository.DeleteAsync(id);
    }
}