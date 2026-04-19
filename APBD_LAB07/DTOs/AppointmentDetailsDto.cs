namespace LAB_07.DTOs;

public class AppointmentDetailsDto
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string Status { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string notes { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
}