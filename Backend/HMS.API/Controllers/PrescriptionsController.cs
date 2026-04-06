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
public class PrescriptionsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PrescriptionsController> _logger;

    public PrescriptionsController(IUnitOfWork unitOfWork, ILogger<PrescriptionsController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? patientId, [FromQuery] int? doctorId, [FromQuery] bool? isActive)
    {
        var userRole = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;
        var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

        IEnumerable<Prescription> prescriptions;

        if (patientId.HasValue)
        {
            prescriptions = await _unitOfWork.Prescriptions.FindAsync(p => p.PatientId == patientId.Value);
        }
        else if (doctorId.HasValue)
        {
            prescriptions = await _unitOfWork.Prescriptions.FindAsync(p => p.DoctorId == doctorId.Value);
        }
        else if (userRole == "Patient")
        {
            var patient = await _unitOfWork.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient == null)
                return NotFound("Patient record not found");
            
            prescriptions = await _unitOfWork.Prescriptions.FindAsync(p => p.PatientId == patient.Id);
        }
        else if (userRole == "Doctor")
        {
            var doctor = await _unitOfWork.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);
            if (doctor == null)
                return NotFound("Doctor record not found");
            
            prescriptions = await _unitOfWork.Prescriptions.FindAsync(p => p.DoctorId == doctor.Id);
        }
        else
        {
            prescriptions = await _unitOfWork.Prescriptions.GetAllAsync();
        }

        if (isActive.HasValue)
        {
            prescriptions = prescriptions.Where(p => p.IsActive == isActive.Value);
        }

        // Return DTOs to avoid circular reference issues
        var prescriptionDtos = prescriptions.Select(p => new
        {
            p.Id,
            p.PatientId,
            p.DoctorId,
            p.AppointmentId,
            p.MedicationName,
            p.Dosage,
            p.Instructions,
            p.Frequency,
            p.Quantity,
            p.DurationDays,
            p.PrescribedDate,
            p.ExpiryDate,
            p.IsActive,
            p.CreatedAt,
            p.UpdatedAt
        });

        return Ok(prescriptionDtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var prescription = await _unitOfWork.Prescriptions.GetByIdAsync(id);
        if (prescription == null)
            return NotFound();

        // Return DTO to avoid circular reference
        var prescriptionDto = new
        {
            prescription.Id,
            prescription.PatientId,
            prescription.DoctorId,
            prescription.AppointmentId,
            prescription.MedicationName,
            prescription.Dosage,
            prescription.Instructions,
            prescription.Frequency,
            prescription.Quantity,
            prescription.DurationDays,
            prescription.PrescribedDate,
            prescription.ExpiryDate,
            prescription.IsActive,
            prescription.CreatedAt,
            prescription.UpdatedAt
        };

        return Ok(prescriptionDto);
    }

    [HttpPost]
    [Authorize(Roles = "Doctor,Admin")]
    public async Task<IActionResult> Create([FromBody] CreatePrescriptionDto createDto)
    {
        try
        {
            var patient = await _unitOfWork.Patients.GetByIdAsync(createDto.PatientId);
            if (patient == null)
                return NotFound("Patient not found");

            var doctor = await _unitOfWork.Doctors.GetByIdAsync(createDto.DoctorId);
            if (doctor == null)
                return NotFound("Doctor not found");

            var prescription = new Prescription
            {
                PatientId = createDto.PatientId,
                DoctorId = createDto.DoctorId,
                AppointmentId = createDto.AppointmentId,
                MedicationName = createDto.MedicationName,
                Dosage = createDto.Dosage,
                Instructions = createDto.Instructions,
                Frequency = createDto.Frequency,
                Quantity = createDto.Quantity,
                DurationDays = createDto.DurationDays,
                PrescribedDate = DateTime.UtcNow,
                ExpiryDate = createDto.DurationDays.HasValue 
                    ? DateTime.UtcNow.AddDays(createDto.DurationDays.Value) 
                    : null,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Prescriptions.AddAsync(prescription);
            await _unitOfWork.SaveChangesAsync();

            // Return DTO to avoid circular reference
            var prescriptionDto = new
            {
                prescription.Id,
                prescription.PatientId,
                prescription.DoctorId,
                prescription.AppointmentId,
                prescription.MedicationName,
                prescription.Dosage,
                prescription.Instructions,
                prescription.Frequency,
                prescription.Quantity,
                prescription.DurationDays,
                prescription.PrescribedDate,
                prescription.ExpiryDate,
                prescription.IsActive,
                prescription.CreatedAt,
                prescription.UpdatedAt
            };

            return CreatedAtAction(nameof(GetById), new { id = prescription.Id }, prescriptionDto);
        }
        catch (DbUpdateException dbEx)
        {
            // Log FULL exception details
            _logger.LogError(dbEx, "=== DATABASE ERROR CREATING PRESCRIPTION ===");
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
                message = "Database error occurred while creating prescription",
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
            _logger.LogError(ex, "=== GENERAL ERROR CREATING PRESCRIPTION ===");
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
                message = "An error occurred while creating the prescription",
                exceptionType = ex.GetType().FullName,
                exceptionMessage = ex.Message,
                stackTrace = ex.StackTrace,
                innerException = ex.InnerException?.Message
            });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Doctor,Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] CreatePrescriptionDto updateDto)
    {
        var prescription = await _unitOfWork.Prescriptions.GetByIdAsync(id);
        if (prescription == null)
            return NotFound();

        prescription.MedicationName = updateDto.MedicationName;
        prescription.Dosage = updateDto.Dosage;
        prescription.Instructions = updateDto.Instructions;
        prescription.Frequency = updateDto.Frequency;
        prescription.Quantity = updateDto.Quantity;
        prescription.DurationDays = updateDto.DurationDays;
        prescription.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Prescriptions.UpdateAsync(prescription);
        await _unitOfWork.SaveChangesAsync();

        return Ok(prescription);
    }

    [HttpPut("{id}/deactivate")]
    [Authorize(Roles = "Doctor,Admin")]
    public async Task<IActionResult> Deactivate(int id)
    {
        var prescription = await _unitOfWork.Prescriptions.GetByIdAsync(id);
        if (prescription == null)
            return NotFound();

        prescription.IsActive = false;
        prescription.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Prescriptions.UpdateAsync(prescription);
        await _unitOfWork.SaveChangesAsync();

        return Ok(new { message = "Prescription deactivated" });
    }
}

public class CreatePrescriptionDto
{
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public int? AppointmentId { get; set; }
    public string MedicationName { get; set; } = string.Empty;
    public string? Dosage { get; set; }
    public string? Instructions { get; set; }
    public string? Frequency { get; set; }
    public int? Quantity { get; set; }
    public int? DurationDays { get; set; }
}

