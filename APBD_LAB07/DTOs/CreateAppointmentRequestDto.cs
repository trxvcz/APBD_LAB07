namespace LAB_07.DTOs;

public class CreateAppointmentRequestDto
{
    public int? DoctorId { get; set; }
    public int? PatientId { get; set; }
    public string? Status { get; set; }
    public string? Reason { get; set; }
    public string? ReasonDescription { get; set; }
    public string? DoctorFullName { get; set; }
    public CreatePatientDTO? CreatePatientDto { get; set; }
    
}

