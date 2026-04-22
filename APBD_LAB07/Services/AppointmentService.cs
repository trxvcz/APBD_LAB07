using System.Data;
using APBD_LAB07.DTOs;
using APBD_LAB07.Exceptions;
using APBD_LAB07.Models;
using Microsoft.Data.SqlClient;

namespace APBD_LAB07.Services;

public class AppointmentService(IConfiguration config)
{
    private readonly string _connectionString = config.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Brak Connection Stringa!");

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
    
    public async Task<Appointment> Create(CreateAppointmentRequestDto appointmentRequest)
    {
        var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        
        // Doctor id By name and surname
        int doctorId = -1;
        if (appointmentRequest.IdDoctor == null)
        {   
            if (appointmentRequest.DoctorFullName == null)
            {
                throw new BadRequestException("Fill the doctor Data");
            }
            
            await using var command =
                new SqlCommand("SELECT IdDoctor FROM Doctors WHERE Doctors.LastName = @lastname AND Doctors.FirstName = @firstname",connection);
            
            var lastName = appointmentRequest.DoctorFullName.Split(' ').Last();
            var firstName = appointmentRequest.DoctorFullName.Split(' ').First();

            command.Parameters.Add("@lastname", SqlDbType.NVarChar).Value = lastName;
            command.Parameters.Add("@firstname", SqlDbType.NVarChar).Value = firstName;

            await using var reader = await command.ExecuteReaderAsync();
            if (reader.HasRows)
            {
                while (await reader.ReadAsync())
                {   
                    doctorId =  reader.GetInt32(reader.GetOrdinal("IdDoctor"));
                }

                await reader.CloseAsync();
            }
            else
            {
                await reader.CloseAsync();
                await using var command2 = new SqlCommand("SELECT IdDoctor FROM Doctors WHERE Doctors.FirstName = @lastname AND Doctors.LastName = @firstname", connection);
                
                command.Parameters.Add("@lastname", SqlDbType.NVarChar).Value = firstName;
                command.Parameters.Add("@firstname", SqlDbType.NVarChar).Value = lastName;
                await using var reader2 = await command2.ExecuteReaderAsync();
                while (await reader2.ReadAsync())
                {   
                    doctorId =  reader.GetInt32(reader.GetOrdinal("IdDoctor"));
                }  
                await reader2.CloseAsync();
            }
        }
        else
        {
            doctorId= appointmentRequest.IdDoctor.Value;
        }
        
        // Pacjent id by name and surname
        int patientId = -1;
        if (appointmentRequest.IdPatient == null)
        {
            if (appointmentRequest.PatientFullName == null)
            {
                throw new BadRequestException("Fill the patient Data");
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
                    await reader.CloseAsync();
            }
            else
            {
                await reader.CloseAsync();
                var command2 = new SqlCommand("SELECT IdPatient FROM Patients WHERE Patients.FirstName = @firstName AND Patients.LastName = @lastName",connection);
                
                
                command.Parameters.Add("@firstname", SqlDbType.NVarChar).Value = lastname;
                command.Parameters.Add("@lastname", SqlDbType.NVarChar).Value = firstname;
                
                await using var reader2 = await command2.ExecuteReaderAsync();
                
                if(await reader2.ReadAsync()){
                    patientId =  reader2.GetInt32(reader.GetOrdinal("IdPatient"));
                }
                await reader2.CloseAsync();
            }
        }
        else
        {
            patientId = appointmentRequest.IdPatient.Value;
        }
        
        // Patient id if exist and is Active
        if (!await CheckIfUserActive(connection, "Patients", "IdPatient", patientId))
        {
            throw new NotFoundException("No active patient found");
        }
        
        
        
        // Doctor id if exists and isActive
        if (!await CheckIfUserActive(connection, "Doctors", "IdDoctor",doctorId))
        {
            
        }

        // if doctor or Patient has free time for appointment or appointment in the past
        if (appointmentRequest.AppointmentDate > DateTime.Now)
        {
            var inWorkingHours = appointmentRequest.AppointmentDate.Hour is > 9 and < 17;
            if (!inWorkingHours)
            {
                throw new BadRequestException("Date after working hours are not allowed");
            }
            
            await using var isFreeCommand = new SqlCommand("SELECT IdAppointment From Appointments WHERE AppointmentDate BETWEEN @startDate AND @endDate AND IdPatient = @idPatient",connection);
            isFreeCommand.Parameters.Add("@startDate", SqlDbType.DateTime).Value = appointmentRequest.AppointmentDate;
            isFreeCommand.Parameters.Add("@endDate", SqlDbType.DateTime).Value = appointmentRequest.AppointmentDate + new TimeSpan(0,1,0,0);
            isFreeCommand.Parameters.Add("@idPatient", SqlDbType.Int).Value = patientId;
            
            await using var reader = await isFreeCommand.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                throw new ConflictException("Patient has Appointment on this date with id: " + reader.GetInt32(reader.GetOrdinal("IdAppointment")));
            }
            
            await reader.CloseAsync();
            
            await using var isFreeDoctorCommand = new SqlCommand("SELECT IdAppointment FROM Appointments WHERE AppointmentDate BETWEEN @startDate AND @endDate AND IdDoctor = @idDoctor;",connection);
            isFreeDoctorCommand.Parameters.Add("@startDate", SqlDbType.DateTime).Value = appointmentRequest.AppointmentDate;
            isFreeDoctorCommand.Parameters.Add("@endDate", SqlDbType.DateTime).Value = appointmentRequest.AppointmentDate + new TimeSpan(0,1,0,0);
            isFreeDoctorCommand.Parameters.Add("@idDoctor", SqlDbType.Int).Value = doctorId;
            
            await using var reader2 = await isFreeDoctorCommand.ExecuteReaderAsync();

            if (await reader2.ReadAsync())
            {
                throw new ConflictException("Doctor has Appointment on this date with id: " + reader2.GetInt32(reader2.GetOrdinal("IdAppointment")));
            }
            await reader2.CloseAsync();
        }
        else
        {
            throw new BadRequestException("Appointment Date must be in the future");
        }
        
        //new id 
        await using var commandId = new SqlCommand( "SELECT MAX(Appointments.IdAppointment)+1 FROM Appointments;",connection);
        var id = 1;
        await using var readerId = await commandId.ExecuteReaderAsync();
        if (await readerId.ReadAsync()&& !readerId.IsDBNull(0))
        {
            id = readerId.GetInt32(0);
        }
        
        await readerId.CloseAsync();
        
        

        var appointment = new Appointment
        {
            Id = id,
            AppointmentDate = appointmentRequest.AppointmentDate,
            CreatedAt = DateTime.Now,
            DoctorId = doctorId,
            PatientId = patientId,
            Status = "Scheduled",
            Reason = appointmentRequest.Reason,
            InternalNotes = null
        };
        
        await using var commandAdd = new SqlCommand("INSERT INTO Appointments(IDPATIENT, IDDOCTOR, APPOINTMENTDATE, STATUS, REASON, INTERNALNOTES, CREATEDAT) Values (@idPatient, @idDoctor, @appointmentDate, @status, @reason, @internalNotes, @CreatedAt);", connection);
        
        commandAdd.Parameters.Add("@idPatient", SqlDbType.Int).Value = appointment.PatientId;
        commandAdd.Parameters.Add("@idDoctor", SqlDbType.Int).Value = appointment.DoctorId;
        commandAdd.Parameters.Add("@appointmentDate", SqlDbType.DateTime).Value = appointment.AppointmentDate;
        commandAdd.Parameters.Add("@status", SqlDbType.NVarChar).Value = appointment.Status;
        commandAdd.Parameters.Add("@reason", SqlDbType.NVarChar).Value = appointment.Reason;
        commandAdd.Parameters.Add("@internalNotes", SqlDbType.NVarChar).Value = (object?)appointment.InternalNotes ?? DBNull.Value;
        commandAdd.Parameters.Add("@CreatedAt", SqlDbType.DateTime).Value = appointment.CreatedAt;

        var readerAdd = await commandAdd.ExecuteNonQueryAsync();

        return readerAdd > 0 ? appointment : throw new BadRequestException("Error occured while adding new appointment");
    }
    

    public async Task<object?> Put(int id, UpdateAppointmentRequestDto appointmentRequest)
    {
        var validStatuses = new[] { "Scheduled", "Completed", "Cancelled" };

        if (!validStatuses.Contains(appointmentRequest.Status))
        {
            throw new BadRequestException("Invalid appointment status");
        }
        
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        
        //obecna wizywta
        await using var checkCommand = new SqlCommand("SELECT AppointmentDate, Status FROM Appointments WHERE IdAppointment = @Id", connection);
        checkCommand.Parameters.Add("@Id", SqlDbType.Int).Value = id;
        
        await using var reader = await checkCommand.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            throw new NotFoundException("Appointment not found");
        }
        
        var currentStatus = reader.GetString(reader.GetOrdinal("Status"));
        var currentDate = reader.GetDateTime(reader.GetOrdinal("AppointmentDate"));
        await reader.CloseAsync();

        if (currentStatus == "Completed" && currentDate == appointmentRequest.AppointmentDate)
        {
            throw new ConflictException("Appointment is completed");
        }


        if (!await CheckIfUserActive(connection, "Patients", "IdPatient", appointmentRequest.IdPatient)) 
        {
            throw new NotFoundException("Patient not existing or is inactive");
        }

        if (!await CheckIfUserActive(connection, "Doctors", "IdDoctor", appointmentRequest.IdDoctor))
        {
            throw new NotFoundException("Doctor not existing or is inactive");
        }

        if (currentDate != appointmentRequest.AppointmentDate && await CheckDoctorScheduleConflict(connection, appointmentRequest.IdDoctor, appointmentRequest.AppointmentDate, id))
           throw new ConflictException("Lekarz ma już wizytę w tym nowym terminie.");
        
        await using var updateCommand = new SqlCommand("UPDATE Appointments SET IdPatient = @IdPatient,IdDoctor = @IdDoctor,AppointmentDate = @AppointmentDate,Status = @Status,Reason = @Reason,InternalNotes = @InternalNotes WHERE IdAppointment = @IdAppointment;",connection);
        updateCommand.Parameters.Add("@IdAppointment", SqlDbType.Int).Value = id;
        updateCommand.Parameters.Add("@IdPatient",  SqlDbType.Int).Value = appointmentRequest.IdPatient;
        updateCommand.Parameters.Add("@IdDoctor", SqlDbType.Int).Value = appointmentRequest.IdDoctor;
        updateCommand.Parameters.Add("@AppointmentDate", SqlDbType.DateTime).Value = appointmentRequest.AppointmentDate;
        updateCommand.Parameters.Add("@Status", SqlDbType.NVarChar).Value = appointmentRequest.Status;
        updateCommand.Parameters.Add("@Reason", SqlDbType.NVarChar).Value = appointmentRequest.Reason;
        updateCommand.Parameters.Add("@InternalNotes", SqlDbType.NVarChar).Value = appointmentRequest.InternalNotes;
        
        
        var read = await updateCommand.ExecuteNonQueryAsync();
        
        return read;
    }

    public async Task<object> DeleteAppointment(int id)
    {
        await using var connection = new SqlConnection(_connectionString);
        await using var checkCommand = new SqlCommand("SELECT Status FROM Appointments WHERE IdAppointment = @Id", connection);
        checkCommand.Parameters.Add("@Id", SqlDbType.Int).Value = id;

        await connection.OpenAsync();
        var statusObj = await checkCommand.ExecuteScalarAsync();

        if (statusObj == null)
            throw new NotFoundException($"Nie znaleziono wizyty o ID {id}.");

        var status = (string)statusObj;
        if (status == "Completed")  
            throw new ConflictException("Nie można usunąć wizyty, która została już zakończona (Completed).");

        await using var deleteCommand = new SqlCommand("DELETE FROM Appointments WHERE IdAppointment = @Id", connection);
        deleteCommand.Parameters.Add("@Id", SqlDbType.Int).Value = id;
        
        var read = await deleteCommand.ExecuteNonQueryAsync();

        return read;
    }
    
    
    
    private static async Task<bool> CheckIfUserActive(SqlConnection connection, string tableName, string idColumnName, int id)
    {
        var query = $"SELECT COUNT(1) FROM {tableName} WHERE {idColumnName} = @Id AND IsActive = 1";
        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
        
        var result = (int)await command.ExecuteScalarAsync()!;
        return result > 0;
    }
    
    private static async Task<bool> CheckDoctorScheduleConflict(SqlConnection connection, int idDoctor, DateTime appointmentDate, int? idAppointmentToExclude)
    {
        var query = """
                    SELECT COUNT(1) FROM Appointments 
                    WHERE IdDoctor = @IdDoctor 
                      AND AppointmentDate = @AppointmentDate
                      AND Status != 'Cancelled'
                    """;

        if (idAppointmentToExclude.HasValue)
        {
            query += " AND IdAppointment != @ExcludeId";
        }

        await using var command = new SqlCommand(query, connection);
        command.Parameters.Add("@IdDoctor", SqlDbType.Int).Value = idDoctor;
        command.Parameters.Add("@AppointmentDate", SqlDbType.DateTime2).Value = appointmentDate;
        
        if (idAppointmentToExclude.HasValue)
        {
            command.Parameters.Add("@ExcludeId", SqlDbType.Int).Value = idAppointmentToExclude.Value;
        }

        var result = (int)await command.ExecuteScalarAsync()!;
        return result > 0;
    }
}