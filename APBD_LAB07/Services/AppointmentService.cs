using System.Data;
using APBD_LAB07.DTOs;
using APBD_LAB07.Models;
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

        while (await reader.ReadAsync())
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

    public async Task<AppointmentDetailsDto?> GetById(int id)
    {
        await using var connection = new SqlConnection(_connectionString);

        await connection.OpenAsync();
        await using var command =
            new SqlCommand(
                "SELECT a.CreatedAt,D.FirstName + N' ' + D.LastName AS DoctorFullName, D.LicenseNumber,p.Email,p.FirstName + N' ' + p.LastName AS PatientFullName ,p.IdPatient,a.IdAppointment,a.Status,a.Reason,p.PhoneNumber, a.AppointmentDate,a.InternalNotes  FROM Appointments a JOIN Patients P on a.IdPatient = P.IdPatient JOIN Doctors D ON a.IdDoctor = D.IdDoctor WHERE a.IdAppointment= @id",
                
                
                connection);
        command.Parameters.Add("@id",SqlDbType.Int).Value = id;

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync()) return null;
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
        return a;
    }
    
    
   

    public async Task Create(CreateAppointmentRequestDto appointmentRequest)
    {
        var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        
        // Doctor id By name and surname
        int doctorId = -1;
        if (appointmentRequest.DoctorId == null)
        {
            if (appointmentRequest.DoctorFullName == null)
            {
                throw new Exception("Fill the doctor Data");
            }
            
            
            
            await using var command =
                new SqlCommand("SELECT IdDoctor FROM Doctors WHERE Doctors.FirstName = @lastname AND Doctors.FirstName = @firstname",connection);
            
            var lastName = appointmentRequest.DoctorFullName.Split(' ').Last();
            var firstName = appointmentRequest.DoctorFullName.Split(' ').First();

            command.Parameters.Add("@lastname", SqlDbType.NVarChar).Value = lastName;
            command.Parameters.Add("@firstname", SqlDbType.NVarChar).Value = firstName;

            await using var reader = await command.ExecuteReaderAsync();
            if (reader.HasRows)
            {
                while (reader.Read())
                {   
                    doctorId =  reader.GetInt32(reader.GetOrdinal("IdDoctor"));
                }    
            }
            else
            {
                await using var command2 = new SqlCommand("SELECT IdDoctor FROM Doctors WHERE Doctors.FirstName = @lastname AND Doctors.FirstName = @firstname", connection);
                
                command.Parameters.Add("@lastname", SqlDbType.NVarChar).Value = firstName;
                command.Parameters.Add("@firstname", SqlDbType.NVarChar).Value = lastName;
                
                while (reader.Read())
                {   
                    doctorId =  reader.GetInt32(reader.GetOrdinal("IdDoctor"));
                }  
            }
        }
        else
        {
            doctorId= appointmentRequest.DoctorId.Value;
        }
        
        // Pacjent id by name and surname
        int patientId = -1;
        if (appointmentRequest.PatientId == null)
        {
            if (appointmentRequest.PatientFullName == null)
            {
                throw new Exception("Fill the patient Data");
            }
            
            await using var command = new SqlCommand("SELECT IdPatient FROM Patients WHERE Patients.FirstName = @firstName AND Patients.LastName = @lastName",connection);
            
            var firstname =  appointmentRequest.PatientFullName.Split(' ').First();
            var lastname = appointmentRequest.PatientFullName.Split(' ').Last();

            command.Parameters.Add("@firstname", SqlDbType.NVarChar).Value = firstname;
            command.Parameters.Add("@lastname", SqlDbType.NVarChar).Value = lastname;
            
            await using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                    patientId= reader.GetInt32(reader.GetOrdinal("IdPatient"));
            }
            else
            {
                var command2 = new SqlCommand("SELECT IdPatient FROM Patients WHERE Patients.FirstName = @firstName AND Patients.LastName = @lastName",connection);
                
                
                command.Parameters.Add("@firstname", SqlDbType.NVarChar).Value = lastname;
                command.Parameters.Add("@lastname", SqlDbType.NVarChar).Value = firstname;
                
                await using var reader2 = await command2.ExecuteReaderAsync();
                
                if(await reader2.ReadAsync()){
                    patientId =  reader2.GetInt32(reader.GetOrdinal("IdPatient"));
                }
            }
        }
        else
        {
            patientId = appointmentRequest.PatientId.Value;
        }
        
        // Patient id if exist and is Active
        if (appointmentRequest.PatientId != null)
        {
            await using var existAndActiveCommand = new SqlCommand("SELECT IdPatient From Patients WHERE IdPatient=1 AND IsActive='true'; ", connection);
            existAndActiveCommand.Parameters.Add("@id", SqlDbType.Int).Value = appointmentRequest.PatientId.Value;

            await using var reader = await existAndActiveCommand.ExecuteReaderAsync();


            if (!await reader.ReadAsync())
            {
                throw new Exception("No active patient found");
            }
        }
        
        
        
        //TODO DoctorId exists and is Active

        var commandId = new SqlCommand( "SELECT MAX(Appointments.IdAppointment)+1 FROM Appointments;",connection);
        int id = 0;
        var readerId = commandId.ExecuteReader();
        if (readerId.Read())
        {
            id = readerId.GetInt32(0);
        }
        
        
        

        var appointment = new Appointment
        {
            Id = id,
            AppointmentDate = appointmentRequest.AppointmentDate,
            CreatedAt = DateTime.Now,
            DoctorId = doctorId,
            PatientId = patientId,
            Status = "",
            Reason = appointmentRequest.Reason,
            InternalNotes = ""
        };
        
        
        var commandAdd = new SqlCommand("",connection);
        












    }
}