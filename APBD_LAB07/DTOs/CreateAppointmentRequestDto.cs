namespace APBD_LAB07.DTOs;

public class CreateAppointmentRequestDto
{
    public int? IdDoctor { get; set; }
    public int? IdPatient { get; set; }
    public string? DoctorFullName { get; set; }
    public string? PatientFullName { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string Reason { get; set; }
}