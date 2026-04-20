namespace APBD_LAB07.Models;

public class Appointment
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string Status { get; set; }
    public string Reason { get; set; }
    public string InternalNotes { get; set; }
    public DateTime CreatedAt { get; set; }
}