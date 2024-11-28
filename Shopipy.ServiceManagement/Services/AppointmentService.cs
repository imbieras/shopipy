using Shopipy.Persistence.Models;
using Shopipy.Persistence.Repositories;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;

namespace Shopipy.ServiceManagement.Services;

public class AppointmentService(
    IGenericRepository<Appointment> appointmentRepository,
    IGenericRepository<Service> serviceRepository,
    IGenericRepository<User> userRepository
)
{

    public async Task<IEnumerable<Appointment>> GetAllAppointments()
    {
        return await appointmentRepository.GetAllAsync();
    }

    public async Task<IEnumerable<Appointment>> GetAllAppointmentsInBusinessAsync(int businessId)
    {
        return await appointmentRepository.GetAllByConditionAsync(a => a.BusinessId == businessId);
    }
    
    public async Task<IEnumerable<Service>> GetServicesByIdsAsync(IEnumerable<int> serviceIds)
    {
        return await serviceRepository.GetAllByConditionAsync(s => serviceIds.Contains(s.ServiceId));
    }

    public async Task<List<User>> GetAvailableEmployees(int businessId, DateTime time, int serviceId)
    {
        var service = await serviceRepository.GetByConditionAsync(s => s.ServiceId == serviceId);
        
        if (service == null)
        {
            throw new Exception($"Service with ID {serviceId} does not exist");
        }

        var employees = await userRepository.GetAllByConditionAsync(u =>
            u.BusinessId == businessId && 
            u.UserState == UserState.Active);

        var appointmentEndTime = time.AddMinutes(service.ServiceDuration);

        var filteredConflictingAppointments = await appointmentRepository.GetAllByConditionAsync(a =>
            a.BusinessId == businessId &&
            a.ServiceId == serviceId &&
            (
                (a.StartTime <= time && a.EndTime > time) ||
                (a.StartTime < appointmentEndTime && a.StartTime >= time) 
            ));

        var conflictingEmployeeIds = filteredConflictingAppointments
            .Select(a => a.EmployeeId)
            .ToHashSet();

        try 
        {
            var availableEmployees = employees
                .Where(e => 
                {
                    try 
                    {
                        var parsedId = Guid.Parse(e.Id);
                        var isAvailable = !conflictingEmployeeIds.Contains(parsedId);
                        return isAvailable;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Debug] Error parsing ID {e.Id}: {ex.Message}");
                        return false;
                    }
                })
                .ToList();

            return availableEmployees;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Debug] Error in final filtering: {ex.Message}");
            throw;
        }
    }
    
    //krc kentai musu is kinijos, gali 24h dirbt
    public async Task<List<DateTime>> GetAvailableTimeSlots(int businessId, Guid employeeId, DateTime date, int serviceId)
    {
        var dayStart = date.Date;
        var dayEnd = dayStart.AddDays(1);
    
        var service = await serviceRepository.GetByConditionAsync(s => s.BusinessId == businessId && s.ServiceId == serviceId);
        if (service == null)
            throw new InvalidOperationException($"Service with ID {serviceId} not found");
        
        var serviceDuration = service.ServiceDuration;
        if (serviceDuration <= 0)
            throw new InvalidOperationException("Service duration must be greater than 0");
    
        var appointments = await appointmentRepository.GetAllByConditionAsync(a =>
            a.BusinessId == businessId &&
            a.EmployeeId == employeeId &&
            a.StartTime.Date == date.Date) ?? new List<Appointment>();

        var busyPeriods = appointments
            .OrderBy(a => a.StartTime)
            .Select(a => new { Start = a.StartTime, End = a.EndTime })
            .ToList();

        busyPeriods.Insert(0, new { Start = dayStart, End = dayStart });
        busyPeriods.Add(new { Start = dayEnd, End = dayEnd });

        return busyPeriods
            .Zip(busyPeriods.Skip(1), (current, next) => new { Start = current.End, End = next.Start })
            .Where(gap => gap.End > gap.Start && (gap.End - gap.Start).TotalMinutes >= serviceDuration)
            .SelectMany(gap => 
                Enumerable
                    .Range(0, (int)((gap.End - gap.Start).TotalMinutes / serviceDuration))
                    .Select(i => gap.Start.AddMinutes(i * serviceDuration)))
            .ToList();
    }

    
    public async Task<IEnumerable<Appointment>> GetAppointmentsOfEmployee(int businessId, Guid employeeId, DateTime time, bool week)
    {
        var dateStart = time.Date;
        var dateEnd = dateStart.AddDays(1); 
        var weekEnd = dateStart.AddDays(7);
    
        var appointments = await appointmentRepository.GetAllByConditionAsync(
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
        return await appointmentRepository.GetByIdAsync(id);
    }

    public async Task<Appointment> GetAppointmentByIdInBusinessAsync(int businessId, int id)
    {
        return await appointmentRepository.GetByConditionAsync(a => a.BusinessId == businessId && a.AppointmentId == id);
    }

    public async Task<Appointment> CreateAppointmentAsync(Appointment appointment)
    {
        var service = await serviceRepository.GetByIdAsync(appointment.ServiceId);
        
        if (service == null)
        {
            throw new ArgumentException("Service not found");
        }

        appointment.EndTime = appointment.StartTime.AddMinutes(service.ServiceDuration);

        return await appointmentRepository.AddAsync(appointment);
    }

    public async Task<Appointment> UpdateAppointmentAsync(Appointment appointment)
    {
        return await appointmentRepository.UpdateAsync(appointment);
    }

    public async Task<bool> DeleteAppointmentAsync(int id)
    {
        return await appointmentRepository.DeleteAsync(id);
    }
}