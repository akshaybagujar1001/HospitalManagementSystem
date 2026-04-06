using HMS.Domain.Entities;
using HMS.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AdmissionsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AdmissionsController> _logger;

    public AdmissionsController(IUnitOfWork unitOfWork, ILogger<AdmissionsController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? patientId, [FromQuery] string? status)
    {
        IEnumerable<Admission> admissions;

        if (patientId.HasValue)
        {
            admissions = await _unitOfWork.Admissions.FindAsync(a => a.PatientId == patientId.Value);
        }
        else
        {
            admissions = await _unitOfWork.Admissions.GetAllAsync();
        }

        if (!string.IsNullOrEmpty(status))
        {
            admissions = admissions.Where(a => a.Status == status);
        }

        return Ok(admissions.OrderByDescending(a => a.AdmissionDate));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var admission = await _unitOfWork.Admissions.GetByIdAsync(id);
        if (admission == null)
            return NotFound();

        return Ok(admission);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Doctor,Nurse")]
    public async Task<IActionResult> Admit([FromBody] CreateAdmissionDto createDto)
    {
        try
        {
            var patient = await _unitOfWork.Patients.GetByIdAsync(createDto.PatientId);
            if (patient == null)
                return NotFound("Patient not found");

            var bed = await _unitOfWork.Beds.GetByIdAsync(createDto.BedId);
            if (bed == null)
                return NotFound("Bed not found");

            if (bed.IsOccupied)
                return BadRequest("Bed is already occupied");

            var admission = new Admission
            {
                PatientId = createDto.PatientId,
                BedId = createDto.BedId,
                DoctorId = createDto.DoctorId,
                NurseId = createDto.NurseId,
                AdmissionNumber = $"ADM-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}",
                AdmissionDate = DateTime.UtcNow,
                Status = "Admitted",
                Reason = createDto.Reason,
                Notes = createDto.Notes,
                CreatedAt = DateTime.UtcNow
            };

            // Mark bed as occupied
            bed.IsOccupied = true;
            await _unitOfWork.Beds.UpdateAsync(bed);

            await _unitOfWork.Admissions.AddAsync(admission);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = admission.Id }, admission);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error admitting patient");
            return StatusCode(500, new { message = "An error occurred while admitting the patient" });
        }
    }

    [HttpPut("{id}/discharge")]
    [Authorize(Roles = "Admin,Doctor,Nurse")]
    public async Task<IActionResult> Discharge(int id, [FromBody] DischargeDto dischargeDto)
    {
        var admission = await _unitOfWork.Admissions.GetByIdAsync(id);
        if (admission == null)
            return NotFound();

        if (admission.Status == "Discharged")
            return BadRequest("Patient is already discharged");

        admission.Status = "Discharged";
        admission.DischargeDate = DateTime.UtcNow;
        admission.Notes = dischargeDto.Notes ?? admission.Notes;
        admission.UpdatedAt = DateTime.UtcNow;

        // Free the bed
        var bed = await _unitOfWork.Beds.GetByIdAsync(admission.BedId);
        if (bed != null)
        {
            bed.IsOccupied = false;
            await _unitOfWork.Beds.UpdateAsync(bed);
        }

        await _unitOfWork.Admissions.UpdateAsync(admission);
        await _unitOfWork.SaveChangesAsync();

        return Ok(admission);
    }
}

public class CreateAdmissionDto
{
    public int PatientId { get; set; }
    public int BedId { get; set; }
    public int? DoctorId { get; set; }
    public int? NurseId { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
}

public class DischargeDto
{
    public string? Notes { get; set; }
}

