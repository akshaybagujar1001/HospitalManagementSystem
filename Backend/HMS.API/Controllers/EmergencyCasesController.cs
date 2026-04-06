using HMS.Application.DTOs.Emergency;
using HMS.Domain.Entities;
using HMS.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HMS.API.Controllers;

[ApiController]
[Route("api/emergency-cases")]
[Authorize(Roles = "Admin,Doctor,Nurse")]
public class EmergencyCasesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<EmergencyCasesController> _logger;

    public EmergencyCasesController(IUnitOfWork unitOfWork, ILogger<EmergencyCasesController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetCases([FromQuery] string? status, [FromQuery] int? triageLevel)
    {
        var cases = await _unitOfWork.EmergencyCases.FindAsync(c =>
            (status == null || c.Status == status) &&
            (triageLevel == null || c.TriageLevel == triageLevel)
        );

        var caseDtos = cases.Select(c => new EmergencyCaseDto
        {
            Id = c.Id,
            PatientId = c.PatientId,
            TriageLevel = c.TriageLevel,
            Description = c.Description,
            Status = c.Status,
            ArrivalTime = c.ArrivalTime,
            AssignedDoctorId = c.AssignedDoctorId,
            AssignedNurseId = c.AssignedNurseId
        });

        return Ok(caseDtos);
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveCases()
    {
        var cases = await _unitOfWork.EmergencyCases.FindAsync(c =>
            c.Status != "Discharged" && c.Status != "Admitted"
        );

        return Ok(cases);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCase([FromBody] CreateEmergencyCaseDto createDto)
    {
        try
        {
            var emergencyCase = new EmergencyCase
            {
                PatientId = createDto.PatientId,
                TriageLevel = createDto.TriageLevel,
                Description = createDto.Description,
                ChiefComplaint = createDto.ChiefComplaint,
                Status = "Arrived",
                ArrivalTime = DateTime.UtcNow,
                TriageTime = DateTime.UtcNow
            };

            await _unitOfWork.EmergencyCases.AddAsync(emergencyCase);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCase), new { id = emergencyCase.Id }, emergencyCase);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating emergency case");
            return StatusCode(500, new { message = "An error occurred while creating the emergency case" });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCase(int id)
    {
        var emergencyCase = await _unitOfWork.EmergencyCases.GetByIdAsync(id);
        if (emergencyCase == null)
            return NotFound();

        return Ok(emergencyCase);
    }

    [HttpPut("{id}/triage")]
    [Authorize(Roles = "Doctor,Admin")]
    public async Task<IActionResult> UpdateTriage(int id, [FromBody] UpdateTriageDto updateDto)
    {
        var emergencyCase = await _unitOfWork.EmergencyCases.GetByIdAsync(id);
        if (emergencyCase == null)
            return NotFound();

        emergencyCase.TriageLevel = updateDto.TriageLevel;
        emergencyCase.TriageTime = DateTime.UtcNow;
        emergencyCase.Status = "Triage";

        await _unitOfWork.EmergencyCases.UpdateAsync(emergencyCase);
        await _unitOfWork.SaveChangesAsync();

        return Ok(new { message = "Triage updated" });
    }

    [HttpPut("{id}/assign")]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<IActionResult> AssignStaff(int id, [FromBody] AssignStaffDto assignDto)
    {
        var emergencyCase = await _unitOfWork.EmergencyCases.GetByIdAsync(id);
        if (emergencyCase == null)
            return NotFound();

        if (assignDto.DoctorId.HasValue)
            emergencyCase.AssignedDoctorId = assignDto.DoctorId;
        if (assignDto.NurseId.HasValue)
            emergencyCase.AssignedNurseId = assignDto.NurseId;
        if (assignDto.RoomId.HasValue)
            emergencyCase.RoomId = assignDto.RoomId;

        emergencyCase.Status = "Treatment";
        emergencyCase.TreatmentStartTime = DateTime.UtcNow;

        await _unitOfWork.EmergencyCases.UpdateAsync(emergencyCase);
        await _unitOfWork.SaveChangesAsync();

        return Ok(new { message = "Staff assigned" });
    }
}

