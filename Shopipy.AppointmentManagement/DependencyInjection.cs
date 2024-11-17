using AppointmentManagement.Services;

namespace AppointmentManagement;

public static class DependencyInjection
{
    public static IServiceCollection AddAppointmentManagement(this IServiceCollection appointments)
    {
        appointments.AddScoped<AppointmentService>();
        return appointments;
    }
}