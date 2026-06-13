namespace MediFlow.Core.Models;

public class Doctor
{
    public string DoctorId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true;
    public int YearsOfExperience { get; set; }
    public string Qualification { get; set; } = string.Empty;
}
