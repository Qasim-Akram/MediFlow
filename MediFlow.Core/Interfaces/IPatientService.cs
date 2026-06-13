using MediFlow.Core.Models;

namespace MediFlow.Core.Interfaces;

public interface IPatientService
{
    Task<List<Patient>> GetAllAsync();
    Task<List<Patient>> SearchAsync(string keyword, string gender, string bloodGroup);
    Task<Patient?> GetByIdAsync(string id);
    Task AddAsync(Patient patient);
    Task UpdateAsync(Patient patient);
    Task DeleteAsync(string id);
    Task<int> GetTotalCountAsync();
    Task<Dictionary<string, int>> GetGenderStatsAsync();
    Task<Dictionary<string, int>> GetBloodGroupStatsAsync();
}
