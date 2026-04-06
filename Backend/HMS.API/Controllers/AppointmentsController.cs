using AutoMapper;
using HMS.Application.DTOs.Appointment;
using HMS.Domain.Entities;
using HMS.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace HMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<AppointmentsController> _logger;

    public AppointmentsController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<AppointmentsController> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? patientId, [FromQuery] int? doctorId)
    {
        var userRole = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;
        var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

        IEnumerable<Appointment> appointments;

        if (patientId.HasValue)
        {
            appointments = await _unitOfWork.Appointments.FindAsync(a => a.PatientId == patientId.Value);
        }
        else if (doctorId.HasValue)
        {
            appointments = await _unitOfWork.Appointments.FindAsync(a => a.DoctorId == doctorId.Value);
        }
        else if (userRole == "Patient")
        {
            var patient = await _unitOfWork.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient == null)
                return NotFound("Patient record not found");
            
            appointments = await _unitOfWork.Appointments.FindAsync(a => a.PatientId == patient.Id);
        }
        else if (userRole == "Doctor")
        {
            var doctor = await _unitOfWork.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);
            if (doctor == null)
                return NotFound("Doctor record not found");
            
            appointments = await _unitOfWork.Appointments.FindAsync(a => a.DoctorId == doctor.Id);
        }
        else
        {
            appointments = await _unitOfWork.Appointments.GetAllAsync();
        }

        var appointmentDtos = _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
        return Ok(appointmentDtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var appointment = await _unitOfWork.Appointments.GetByIdAsync(id);
        if (appointment == null)
            return NotFound();

        var appointmentDto = _mapper.Map<AppointmentDto>(appointment);
        return Ok(appointmentDto);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAppointmentDto createAppointmentDto)
    {
        try
        {
            var patient = await _unitOfWork.Patients.GetByIdAsync(createAppointmentDto.PatientId);
            if (patient == null)
                return NotFound("Patient not found");

            var doctor = await _unitOfWork.Doctors.GetByIdAsync(createAppointmentDto.DoctorId);
            if (doctor == null)
                return NotFound("Doctor not found");

            if (!doctor.IsAvailable)
                return BadRequest("Doctor is not available");

            var appointment = new Appointment
            {
                PatientId = createAppointmentDto.PatientId,
                DoctorId = createAppointmentDto.DoctorId,
                AppointmentDate = createAppointmentDto.AppointmentDate,
                AppointmentTime = createAppointmentDto.AppointmentTime,
                Status = "Scheduled",
                Reason = createAppointmentDto.Reason,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Appointments.AddAsync(appointment);
            await _unitOfWork.SaveChangesAsync();

            var appointmentDto = _mapper.Map<AppointmentDto>(appointment);
            return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, appointmentDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating appointment");
            return StatusCode(500, new { message = "An error occurred while creating the appointment" });
        }
    }

    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        try
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(id);
            if (appointment == null)
            {
                _logger.LogWarning("Attempted to cancel non-existent appointment {AppointmentId}", id);
                return NotFound(new { message = "Appointment not found" });
            }

            // Validation: Check if appointment can be cancelled
            if (appointment.Status == "Cancelled")
            {
                _logger.LogWarning("Attempted to cancel already cancelled appointment {AppointmentId}", id);
                return BadRequest(new { message = "Appointment is already cancelled" });
            }

            if (appointment.Status == "Completed")
            {
                _logger.LogWarning("Attempted to cancel completed appointment {AppointmentId}", id);
                return BadRequest(new { message = "Cannot cancel a completed appointment" });
            }

            // Update appointment status
            appointment.Status = "Cancelled";
            appointment.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Appointments.UpdateAsync(appointment);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Appointment {AppointmentId} cancelled successfully", id);
            return Ok(new { message = "Appointment cancelled successfully" });
        }
        catch (DbUpdateException dbEx)
        {
            // Log FULL exception details for debugging
            _logger.LogError(dbEx, "=== DATABASE ERROR CANCELLING APPOINTMENT {AppointmentId} ===", id);
            _logger.LogError("Exception Type: {Type}", dbEx.GetType().FullName);
            _logger.LogError("Exception Message: {Message}", dbEx.Message);
            _logger.LogError("Stack Trace: {StackTrace}", dbEx.StackTrace);
            
            // Log inner exception details
            var innerEx = dbEx.InnerException;
            int depth = 0;
            while (innerEx != null)
            {
                depth++;
                _logger.LogError("Inner Exception [{Depth}]: {Type} - {Message}", depth, innerEx.GetType().FullName, innerEx.Message);
                _logger.LogError("Inner Exception [{Depth}] Stack Trace: {StackTrace}", depth, innerEx.StackTrace);
                
                // Check for SqlException
                if (innerEx is Microsoft.Data.SqlClient.SqlException sqlEx)
                {
                    _logger.LogError("SQL Exception Number: {Number}", sqlEx.Number);
                    _logger.LogError("SQL Exception State: {State}", sqlEx.State);
                    _logger.LogError("SQL Exception Class: {Class}", sqlEx.Class);
                    _logger.LogError("SQL Exception Server: {Server}", sqlEx.Server);
                    _logger.LogError("SQL Exception Procedure: {Procedure}", sqlEx.Procedure);
                    _logger.LogError("SQL Exception LineNumber: {LineNumber}", sqlEx.LineNumber);
                    _logger.LogError("SQL Errors Count: {Count}", sqlEx.Errors.Count);
                    for (int i = 0; i < sqlEx.Errors.Count; i++)
                    {
                        var error = sqlEx.Errors[i];
                        _logger.LogError("SQL Error [{Index}]: Number={Number}, State={State}, Class={Class}, Message={Message}, Source={Source}", 
                            i, error.Number, error.State, error.Class, error.Message, error.Source);
                    }
                }
                
                innerEx = innerEx.InnerException;
            }
            
            // Log entity state if available
            if (dbEx.Entries != null && dbEx.Entries.Any())
            {
                foreach (var entry in dbEx.Entries)
                {
                    _logger.LogError("Entity: {EntityType}, State: {State}", entry.Entity.GetType().Name, entry.State);
                    foreach (var prop in entry.Properties)
                    {
                        _logger.LogError("  Property: {Property} = {Value} (IsModified: {IsModified}, IsTemporary: {IsTemporary})", 
                            prop.Metadata.Name, prop.CurrentValue, prop.IsModified, prop.IsTemporary);
                    }
                }
            }
            
            _logger.LogError("=== END DATABASE ERROR ===");
            
            // Return detailed error
            var errorDetails = new
            {
                message = "Database error occurred while cancelling appointment",
                exceptionType = dbEx.GetType().FullName,
                exceptionMessage = dbEx.Message,
                stackTrace = dbEx.StackTrace,
                innerException = dbEx.InnerException?.Message,
                sqlException = dbEx.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx2 
                    ? new
                    {
                        number = sqlEx2.Number,
                        state = sqlEx2.State,
                        class_ = sqlEx2.Class,
                        message = sqlEx2.Message,
                        server = sqlEx2.Server,
                        errors = sqlEx2.Errors.Cast<Microsoft.Data.SqlClient.SqlError>()
                            .Select(e => new { number = e.Number, state = e.State, message = e.Message, source = e.Source })
                            .ToList()
                    }
                    : null
            };
            
            return StatusCode(500, errorDetails);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "=== GENERAL ERROR CANCELLING APPOINTMENT {AppointmentId} ===", id);
            _logger.LogError("Exception Type: {Type}", ex.GetType().FullName);
            _logger.LogError("Exception Message: {Message}", ex.Message);
            _logger.LogError("Stack Trace: {StackTrace}", ex.StackTrace);
            
            var innerEx = ex.InnerException;
            int depth = 0;
            while (innerEx != null)
            {
                depth++;
                _logger.LogError("Inner Exception [{Depth}]: {Type} - {Message}", depth, innerEx.GetType().FullName, innerEx.Message);
                _logger.LogError("Inner Exception [{Depth}] Stack Trace: {StackTrace}", depth, innerEx.StackTrace);
                innerEx = innerEx.InnerException;
            }
            _logger.LogError("=== END GENERAL ERROR ===");
            
            return StatusCode(500, new 
            { 
                message = "An error occurred while cancelling the appointment",
                exceptionType = ex.GetType().FullName,
                exceptionMessage = ex.Message,
                stackTrace = ex.StackTrace,
                innerException = ex.InnerException?.Message
            });
        }
    }

    [HttpPut("{id}/reschedule")]
    public async Task<IActionResult> Reschedule(int id, [FromBody] CreateAppointmentDto rescheduleDto)
    {
        var appointment = await _unitOfWork.Appointments.GetByIdAsync(id);
        if (appointment == null)
            return NotFound();

        if (appointment.Status == "Completed")
            return BadRequest("Cannot reschedule a completed appointment");

        appointment.AppointmentDate = rescheduleDto.AppointmentDate;
        appointment.AppointmentTime = rescheduleDto.AppointmentTime;
        appointment.Status = "Rescheduled";
        appointment.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Appointments.UpdateAsync(appointment);
        await _unitOfWork.SaveChangesAsync();

        var appointmentDto = _mapper.Map<AppointmentDto>(appointment);
        return Ok(appointmentDto);
    }

    [HttpPut("{id}/complete")]
    [Authorize(Roles = "Doctor,Admin")]
    public async Task<IActionResult> Complete(int id)
    {
        try
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(id);
            if (appointment == null)
            {
                _logger.LogWarning("Attempted to complete non-existent appointment {AppointmentId}", id);
                return NotFound(new { message = "Appointment not found" });
            }

            // Validation: Check if appointment can be completed
            if (appointment.Status == "Completed")
            {
                _logger.LogWarning("Attempted to complete already completed appointment {AppointmentId}", id);
                return BadRequest(new { message = "Appointment is already completed" });
            }

            if (appointment.Status == "Cancelled")
            {
                _logger.LogWarning("Attempted to complete cancelled appointment {AppointmentId}", id);
                return BadRequest(new { message = "Cannot complete a cancelled appointment" });
            }

            // Update appointment status
            appointment.Status = "Completed";
            appointment.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Appointments.UpdateAsync(appointment);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Appointment {AppointmentId} marked as completed", id);
            return Ok(new { message = "Appointment marked as completed" });
        }
        catch (DbUpdateException dbEx)
        {
            // Log FULL exception details for debugging
            _logger.LogError(dbEx, "=== DATABASE ERROR COMPLETING APPOINTMENT {AppointmentId} ===", id);
            _logger.LogError("Exception Type: {Type}", dbEx.GetType().FullName);
            _logger.LogError("Exception Message: {Message}", dbEx.Message);
            _logger.LogError("Stack Trace: {StackTrace}", dbEx.StackTrace);
            
            // Log inner exception details
            var innerEx = dbEx.InnerException;
            int depth = 0;
            while (innerEx != null)
            {
                depth++;
                _logger.LogError("Inner Exception [{Depth}]: {Type} - {Message}", depth, innerEx.GetType().FullName, innerEx.Message);
                _logger.LogError("Inner Exception [{Depth}] Stack Trace: {StackTrace}", depth, innerEx.StackTrace);
                
                // Check for SqlException
                if (innerEx is Microsoft.Data.SqlClient.SqlException sqlEx)
                {
                    _logger.LogError("SQL Exception Number: {Number}", sqlEx.Number);
                    _logger.LogError("SQL Exception State: {State}", sqlEx.State);
                    _logger.LogError("SQL Exception Class: {Class}", sqlEx.Class);
                    _logger.LogError("SQL Exception Server: {Server}", sqlEx.Server);
                    _logger.LogError("SQL Exception Procedure: {Procedure}", sqlEx.Procedure);
                    _logger.LogError("SQL Exception LineNumber: {LineNumber}", sqlEx.LineNumber);
                    _logger.LogError("SQL Errors Count: {Count}", sqlEx.Errors.Count);
                    for (int i = 0; i < sqlEx.Errors.Count; i++)
                    {
                        var error = sqlEx.Errors[i];
                        _logger.LogError("SQL Error [{Index}]: Number={Number}, State={State}, Class={Class}, Message={Message}, Source={Source}", 
                            i, error.Number, error.State, error.Class, error.Message, error.Source);
                    }
                }
                
                innerEx = innerEx.InnerException;
            }
            
            // Log entity state if available
            if (dbEx.Entries != null && dbEx.Entries.Any())
            {
                foreach (var entry in dbEx.Entries)
                {
                    _logger.LogError("Entity: {EntityType}, State: {State}", entry.Entity.GetType().Name, entry.State);
                    foreach (var prop in entry.Properties)
                    {
                        _logger.LogError("  Property: {Property} = {Value} (IsModified: {IsModified}, IsTemporary: {IsTemporary})", 
                            prop.Metadata.Name, prop.CurrentValue, prop.IsModified, prop.IsTemporary);
                    }
                }
            }
            
            _logger.LogError("=== END DATABASE ERROR ===");
            
            // Return detailed error
            var errorDetails = new
            {
                message = "Database error occurred while completing appointment",
                exceptionType = dbEx.GetType().FullName,
                exceptionMessage = dbEx.Message,
                stackTrace = dbEx.StackTrace,
                innerException = dbEx.InnerException?.Message,
                sqlException = dbEx.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx2 
                    ? new
                    {
                        number = sqlEx2.Number,
                        state = sqlEx2.State,
                        class_ = sqlEx2.Class,
                        message = sqlEx2.Message,
                        server = sqlEx2.Server,
                        errors = sqlEx2.Errors.Cast<Microsoft.Data.SqlClient.SqlError>()
                            .Select(e => new { number = e.Number, state = e.State, message = e.Message, source = e.Source })
                            .ToList()
                    }
                    : null
            };
            
            return StatusCode(500, errorDetails);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "=== GENERAL ERROR COMPLETING APPOINTMENT {AppointmentId} ===", id);
            _logger.LogError("Exception Type: {Type}", ex.GetType().FullName);
            _logger.LogError("Exception Message: {Message}", ex.Message);
            _logger.LogError("Stack Trace: {StackTrace}", ex.StackTrace);
            
            var innerEx = ex.InnerException;
            int depth = 0;
            while (innerEx != null)
            {
                depth++;
                _logger.LogError("Inner Exception [{Depth}]: {Type} - {Message}", depth, innerEx.GetType().FullName, innerEx.Message);
                _logger.LogError("Inner Exception [{Depth}] Stack Trace: {StackTrace}", depth, innerEx.StackTrace);
                innerEx = innerEx.InnerException;
            }
            _logger.LogError("=== END GENERAL ERROR ===");
            
            return StatusCode(500, new 
            { 
                message = "An error occurred while completing the appointment",
                exceptionType = ex.GetType().FullName,
                exceptionMessage = ex.Message,
                stackTrace = ex.StackTrace,
                innerException = ex.InnerException?.Message
            });
        }
    }
}

