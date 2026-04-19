using LAB_07.DTOs;
using Microsoft.Data.SqlClient;

namespace APBD_LAB07.Services;

public class AppointmentService(IConfiguration config)
{
    private readonly string _connectionString = config.GetConnectionString("DefaultConnection") 
                                                ?? throw new InvalidOperationException("Brak Connection Stringa!");

    public  List<AppointmentsListDto> GetAll()
    {   
        var appointmentsDtos = new List<AppointmentsListDto>();
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        using var command = new SqlCommand($"SELECT * FROM Appointments JOIN Patients on Appointments.IdPatient = Patients.IdPatient", connection);

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            var a = new AppointmentsListDto()
            {
                IdAppointment = reader.GetInt32(reader.GetOrdinal("IdAppointment")),
                AppointmentDate = reader.GetDateTime(reader.GetOrdinal("AppointmentDate")),
                Status = (reader.GetString(reader.GetOrdinal("Status"))),
                Reason = reader.GetString(reader.GetOrdinal("Reason")),
                PatientFullName = reader.GetString(reader.GetOrdinal("FirstName"))+" " + reader.GetString(reader.GetOrdinal("LastName")),
                PatientEmail = reader.GetString(reader.GetOrdinal("Email"))
            };
            
            appointmentsDtos.Add(a);
        }
        
        return appointmentsDtos;
    }


    public async Task<object?> GetAll(int? id, DateTime? dateTime, string? status, string? reason, string? patientFullName, string? patientEmail)
    {
        throw new NotImplementedException();
    }
}