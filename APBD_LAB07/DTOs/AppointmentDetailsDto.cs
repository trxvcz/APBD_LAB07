namespace APBD_LAB07.DTOs;

public class AppointmentDetailsDto
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public string PatientFullName { get; set; }
    public string PatientEmail { get; set; }
    public string PatientPhone { get; set; }
    public string DoctorLicenseNr { get; set; }
    public string DoctorFullName { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string Status { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string notes { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
}