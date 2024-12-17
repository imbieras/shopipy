using Shopipy.Persistence.Models;

namespace Shopipy.Shared.Services;

public interface IAppointmentService
{
    Task<IEnumerable<Appointment>> GetAllAppointments();
    Task<IEnumerable<Appointment>> GetAllAppointmentsInBusinessAsync(int businessId);
    Task<IEnumerable<Service>> GetServicesByIdsAsync(IEnumerable<int> serviceIds);
    Task<List<User>> GetAvailableEmployees(int businessId, DateTime time, int serviceId);
    Task<List<DateTime>> GetAvailableTimeSlots(int businessId, Guid employeeId, DateTime date, int serviceId);
    Task<IEnumerable<Appointment>> GetAppointmentsOfEmployee(int businessId, Guid employeeId, DateTime time, bool week);
    Task<Appointment?> GetAppointmentById(int id);
    Task<Appointment?> GetAppointmentByIdInBusinessAsync(int businessId, int id);
    Task<Appointment> CreateAppointmentAsync(Appointment appointment, bool smsNotification);
    Task<Appointment> UpdateAppointmentAsync(Appointment appointment, bool smsNotification);
    Task<bool> DeleteAppointmentAsync(int id, bool smsNotification);
}