using System.Data;
using APBD_LAB07.DTOs;
using Microsoft.Data.SqlClient;

namespace APBD_LAB07.Services;

public class AppointmentService(IConfiguration config)
{
    private readonly string _connectionString = config.GetConnectionString("DefaultConnection")
                                                ?? throw new InvalidOperationException("Brak Connection Stringa!");

    public async Task<List<AppointmentsListDto>> GetAll()
    {
        var appointmentsDtos = new List<AppointmentsListDto>();
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command =
            new SqlCommand("SELECT * FROM Appointments JOIN Patients on Appointments.IdPatient = Patients.IdPatient",
                connection);

        await using var reader = await command.ExecuteReaderAsync();

        while (reader.Read())
        {
            var a = new AppointmentsListDto
            {
                IdAppointment = reader.GetInt32(reader.GetOrdinal("IdAppointment")),
                AppointmentDate = reader.GetDateTime(reader.GetOrdinal("AppointmentDate")),
                Status = reader.GetString(reader.GetOrdinal("Status")),
                Reason = reader.GetString(reader.GetOrdinal("Reason")),
                PatientFullName = reader.GetString(reader.GetOrdinal("FirstName")) + " " +
                                  reader.GetString(reader.GetOrdinal("LastName")),
                PatientEmail = reader.GetString(reader.GetOrdinal("Email"))
            };

            appointmentsDtos.Add(a);
        }

        return appointmentsDtos;
    }


    public async Task<List<AppointmentsListDto>> GetAll(string? status, string? patientLastName)
    {
        var appointmentsDetails = new List<AppointmentsListDto>();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var query = new SqlCommand(@"SELECT
            a.IdAppointment,
            a.AppointmentDate,
            a.Status,
            a.Reason,
            p.FirstName + N' ' + p.LastName AS PatientFullName,
            p.Email AS PatientEmail
            FROM Appointments a
            JOIN Patients p ON p.IdPatient = a.IdPatient
            WHERE (@Status IS NULL OR a.Status = @Status)
            AND (@PatientLastName IS NULL OR p.LastName = @PatientLastName)
            ORDER BY a.AppointmentDate;", connection);

        query.Parameters.Add("@Status", SqlDbType.NVarChar).Value = status;
        query.Parameters.Add("@PatientLastName", SqlDbType.NVarChar).Value = patientLastName;

        await using var reader = await query.ExecuteReaderAsync();

        while (reader.Read())
        {
            var a = new AppointmentsListDto
            {
                AppointmentDate = reader.GetDateTime(reader.GetOrdinal("AppointmentDate")),
                IdAppointment = reader.GetInt32(reader.GetOrdinal("IdAppointment")),
                Status = reader.GetString(reader.GetOrdinal("Status")),
                Reason = reader.GetString(reader.GetOrdinal("Reason")),
                PatientFullName = reader.GetString(reader.GetOrdinal("PatientFullName")),
                PatientEmail = reader.GetString(reader.GetOrdinal("PatientEmail"))
            };
            appointmentsDetails.Add(a);
        }

        return appointmentsDetails;
    }

    public async Task<AppointmentDetailsDto> GetById(int id)
    {
        var appointmentDetails = new AppointmentDetailsDto();

        await using var connection = new SqlConnection(_connectionString);

        await connection.OpenAsync();
        await using var command =
            new SqlCommand(
                "SELECT a.CreatedAt,D.FirstName + N' ' + D.LastName AS DoctorFullName, D.LicenseNumber,p.Email,p.FirstName + N' ' + p.LastName AS PatientFullName ,p.IdPatient,a.IdAppointment,a.Status,a.Reason,p.PhoneNumber, a.AppointmentDate,a.InternalNotes  FROM Appointments a JOIN Patients P on a.IdPatient = P.IdPatient JOIN Doctors D ON a.IdDoctor = D.IdDoctor WHERE a.IdAppointment= @id",
                
                
                connection);
        command.Parameters.Add("@id",SqlDbType.Int).Value = id;

        await using var reader = await command.ExecuteReaderAsync();

        while (reader.Read())
        {
            var a = new AppointmentDetailsDto
            {
                AppointmentDate = reader.GetDateTime(reader.GetOrdinal("AppointmentDate")),
                CreatedOn = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                DoctorFullName = reader.GetString(reader.GetOrdinal("DoctorFullName")),
                DoctorLicenseNr = reader.GetString(reader.GetOrdinal("LicenseNumber")),
                PatientEmail = reader.GetString(reader.GetOrdinal("Email")),
                PatientFullName = reader.GetString(reader.GetOrdinal("PatientFullName")),
                PatientId = reader.GetInt32(reader.GetOrdinal("IdPatient")),
                Id = reader.GetInt32(reader.GetOrdinal("IdAppointment")),
                Status = reader.GetString(reader.GetOrdinal("Status")),
                Reason = reader.GetString(reader.GetOrdinal("Reason")),
                notes = reader.IsDBNull(reader.GetOrdinal("InternalNotes")) ? "" :reader.GetString(reader.GetOrdinal("InternalNotes")),
                PatientPhone = reader.GetString(reader.GetOrdinal("PhoneNumber"))
            };
            appointmentDetails =a ;
        }
        return appointmentDetails;
    }
}