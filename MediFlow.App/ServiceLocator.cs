using MediFlow.Core.Interfaces;
using MediFlow.Core.Services;
using Microsoft.Extensions.Configuration;

namespace MediFlow.App;

public static class ServiceLocator
{
    private static string? _connectionString;

    public static string ConnectionString
    {
        get
        {
            if (_connectionString != null) return _connectionString;
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();
            _connectionString = config.GetConnectionString("MediFlow")!;
            return _connectionString;
        }
    }

    public static IPatientService     PatientService     => new PatientService(ConnectionString);
    public static IDoctorService      DoctorService      => new DoctorService(ConnectionString);
    public static IAppointmentService AppointmentService => new AppointmentService(ConnectionString);
}
