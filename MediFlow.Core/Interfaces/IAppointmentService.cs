using MediFlow.Core.Models;

namespace MediFlow.Core.Interfaces;

public interface IAppointmentService
{
    Task<List<Appointment>> GetAllAsync();
    Task<List<Appointment>> SearchAsync(string keyword, AppointmentStatus? status, DateTime? date);
    Task<Appointment?> GetByIdAsync(string id);
    Task AddAsync(Appointment appointment);
    Task UpdateAsync(Appointment appointment);
    Task DeleteAsync(string id);
    Task<int> GetTotalCountAsync();
    Task<Dictionary<string, int>> GetStatusStatsAsync();
    Task<Dictionary<string, int>> GetMonthlyStatsAsync();
    Task<int> GetTodaysCountAsync();
}
