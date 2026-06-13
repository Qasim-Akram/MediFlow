using MediFlow.Core.Models;

namespace MediFlow.Core.Interfaces;

public interface IDoctorService
{
    Task<List<Doctor>> GetAllAsync();
    Task<List<Doctor>> SearchAsync(string keyword, string specialization, bool? isAvailable);
    Task<Doctor?> GetByIdAsync(string id);
    Task AddAsync(Doctor doctor);
    Task UpdateAsync(Doctor doctor);
    Task DeleteAsync(string id);
    Task<int> GetTotalCountAsync();
    Task<Dictionary<string, int>> GetSpecializationStatsAsync();
    Task<List<string>> GetSpecializationsAsync();
}
