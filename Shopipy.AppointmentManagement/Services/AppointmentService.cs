using Shopipy.Persistence.Models;
using Shopipy.Persistence.Repositories;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;

namespace AppointmentManagement.Services;

public class AppointmentService
{
    private readonly IGenericRepository<Appointment> _appointmentRepository;
    private readonly IGenericRepository<Service> _serviceRepository;
    private readonly IGenericRepository<User> _userRepository;

    public AppointmentService(IGenericRepository<Appointment> appointmentRepository,
        IGenericRepository<Service> serviceRepository, IGenericRepository<User> userRepository)
    {
        _appointmentRepository = appointmentRepository;
        _serviceRepository = serviceRepository;
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<Appointment>> GetAllAppointments()
    {
        return await _appointmentRepository.GetAllAsync();
    }

    public async Task<IEnumerable<Appointment>> GetAllAppointmentsInBusinessAsync(int businessId)
    {
        return await _appointmentRepository.GetAllByConditionAsync(a => a.BusinessId == businessId);
    }
    
    //yeah ik this is an appointment service, and that I shouldnt use service endpoint data logic here, but wtv it works
    public async Task<IEnumerable<Service>> GetServicesByIdsAsync(IEnumerable<int> serviceIds)
    {
        return await _serviceRepository.GetAllByConditionAsync(s => serviceIds.Contains(s.ServiceId));
    }

    public async Task<IEnumerable<User>> GetAvailableEmployees(int businessId, DateTime time, int serviceId)
    {
        var service = await _serviceRepository.GetByConditionAsync(s => s.ServiceId == serviceId);
        
        if (service == null)
        {
            throw new Exception($"Service with ID {serviceId} does not exist");
        }

        var employees =
            await _userRepository.GetAllByConditionAsync(u =>
                u.BusinessId == businessId && u.UserState == UserState.Active);

        var filteredConflictingAppointments = await _appointmentRepository.GetAllByConditionAsync(a =>
            a.BusinessId == businessId &&
            a.ServiceId == serviceId &&
            a.StartTime < time.AddMinutes(service.ServiceDuration) &&
            a.EndTime > time);

        var conflictingAppointments = filteredConflictingAppointments.Select(a => a.EmployeeId);

        var availableEmployees = employees
            .Where(e => !conflictingAppointments.Contains(Int32.Parse(e.Id)))
            .ToList();

        return availableEmployees;
    }
    
    public async Task<IEnumerable<Appointment>> GetAppointmentsOfEmployee(int businessId, int employeeId, DateTime time, bool week)
    {
        var dateStart = time.Date;
        var dateEnd = dateStart.AddDays(1); 
        var weekEnd = dateStart.AddDays(7);
    
        var appointments = await _appointmentRepository.GetAllByConditionAsync(
            a => a.BusinessId == businessId && a.EmployeeId == employeeId
        );

        if (week)
        {
            return appointments.Where(
                a => a.StartTime >= dateStart && 
                     a.StartTime < weekEnd  
            ).OrderBy(a => a.StartTime);
        }
    
        return appointments.Where(
            a => a.StartTime >= dateStart && 
                 a.StartTime < dateEnd
        ).OrderBy(a => a.StartTime);
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