namespace MediFlow.Core.Models;

public class Patient
{
    public string PatientId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string BloodGroup { get; set; } = string.Empty;
    public DateTime RegisteredOn { get; set; } = DateTime.Now;

    public int Age => DateTime.Today.Year - DateOfBirth.Year -
                      (DateTime.Today.DayOfYear < DateOfBirth.DayOfYear ? 1 : 0);
}
