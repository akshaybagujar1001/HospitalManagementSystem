using HMS.Domain.Entities;
using HMS.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MedicalRecordsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<MedicalRecordsController> _logger;

    public MedicalRecordsController(IUnitOfWork unitOfWork, ILogger<MedicalRecordsController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? patientId, [FromQuery] int? doctorId)
    {
        var userRole = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;
        var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

        IEnumerable<MedicalRecord> records;

        if (patientId.HasValue)
        {
            records = await _unitOfWork.MedicalRecords.FindAsync(r => r.PatientId == patientId.Value);
        }
        else if (doctorId.HasValue)
        {
            records = await _unitOfWork.MedicalRecords.FindAsync(r => r.DoctorId == doctorId.Value);
        }
        else if (userRole == "Patient")
        {
            var patient = await _unitOfWork.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient == null)
                return NotFound("Patient record not found");
            
            records = await _unitOfWork.MedicalRecords.FindAsync(r => r.PatientId == patient.Id);
        }
        else if (userRole == "Doctor")
        {
            var doctor = await _unitOfWork.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);
            if (doctor == null)
                return NotFound("Doctor record not found");
            
            records = await _unitOfWork.MedicalRecords.FindAsync(r => r.DoctorId == doctor.Id);
        }
        else
        {
            records = await _unitOfWork.MedicalRecords.GetAllAsync();
        }

        return Ok(records.OrderByDescending(r => r.RecordDate));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var record = await _unitOfWork.MedicalRecords.GetByIdAsync(id);
        if (record == null)
            return NotFound();

        return Ok(record);
    }

    [HttpPost]
    [Authorize(Roles = "Doctor,Admin")]
    public async Task<IActionResult> Create([FromBody] CreateMedicalRecordDto createDto)
    {
        try
        {
            var patient = await _unitOfWork.Patients.GetByIdAsync(createDto.PatientId);
            if (patient == null)
                return NotFound("Patient not found");

            var doctor = await _unitOfWork.Doctors.GetByIdAsync(createDto.DoctorId);
            if (doctor == null)
                return NotFound("Doctor not found");

            var record = new MedicalRecord
            {
                PatientId = createDto.PatientId,
                DoctorId = createDto.DoctorId,
                AppointmentId = createDto.AppointmentId,
                Diagnosis = createDto.Diagnosis,
                Symptoms = createDto.Symptoms,
                Treatment = createDto.Treatment,
                Notes = createDto.Notes,
                RecordDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.MedicalRecords.AddAsync(record);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = record.Id }, record);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating medical record");
            return StatusCode(500, new { message = "An error occurred while creating the medical record" });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Doctor,Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateMedicalRecordDto updateDto)
    {
        var record = await _unitOfWork.MedicalRecords.GetByIdAsync(id);
        if (record == null)
            return NotFound();

        record.Diagnosis = updateDto.Diagnosis;
        record.Symptoms = updateDto.Symptoms;
        record.Treatment = updateDto.Treatment;
        record.Notes = updateDto.Notes;
        record.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.MedicalRecords.UpdateAsync(record);
        await _unitOfWork.SaveChangesAsync();

        return Ok(record);
    }
}

public class CreateMedicalRecordDto
{
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public int? AppointmentId { get; set; }
    public string Diagnosis { get; set; } = string.Empty;
    public string? Symptoms { get; set; }
    public string? Treatment { get; set; }
    public string? Notes { get; set; }
}

