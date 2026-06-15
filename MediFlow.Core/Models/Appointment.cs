namespace MediFlow.Core.Models;

public enum AppointmentStatus
{
    Scheduled,    Completed, Cancelled
}

public class Appointment
{
    public string AppointmentId { get; set; } = string.Empty;
    public string PatientId { get; set; } = string.Empty;
    public string DoctorId { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public string DoctorName { get; set; } = string.Empty;
    public DateTime AppointmentDate { get; set; }
    public string Reason { get; set; } = string.Empty;
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;
    public string Notes { get; set; } = string.Empty;
    public string TokenNumber { get; set; } = string.Empty;
}
