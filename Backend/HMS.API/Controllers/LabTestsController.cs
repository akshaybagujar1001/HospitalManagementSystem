using AutoMapper;
using HMS.Application.DTOs.Invoice;
using HMS.Domain.Entities;
using HMS.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LabTestsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<LabTestsController> _logger;

    public LabTestsController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<LabTestsController> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet("types")]
    public async Task<IActionResult> GetLabTestTypes()
    {
        var types = await _unitOfWork.LabTestTypes.FindAsync(t => t.IsActive);
        return Ok(types);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? patientId, [FromQuery] int? doctorId, [FromQuery] string? status)
    {
        var userRole = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;
        var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

        IEnumerable<LabTest> labTests;

        if (patientId.HasValue)
        {
            labTests = await _unitOfWork.LabTests.FindAsync(t => t.PatientId == patientId.Value);
        }
        else if (doctorId.HasValue)
        {
            labTests = await _unitOfWork.LabTests.FindAsync(t => t.DoctorId == doctorId.Value);
        }
        else if (userRole == "Patient")
        {
            var patient = await _unitOfWork.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient == null)
                return NotFound("Patient record not found");
            
            labTests = await _unitOfWork.LabTests.FindAsync(t => t.PatientId == patient.Id);
        }
        else if (userRole == "Doctor")
        {
            var doctor = await _unitOfWork.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);
            if (doctor == null)
                return NotFound("Doctor record not found");
            
            labTests = await _unitOfWork.LabTests.FindAsync(t => t.DoctorId == doctor.Id);
        }
        else
        {
            labTests = await _unitOfWork.LabTests.GetAllAsync();
        }

        if (!string.IsNullOrEmpty(status))
        {
            labTests = labTests.Where(t => t.Status == status);
        }

        return Ok(labTests);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var labTest = await _unitOfWork.LabTests.GetByIdAsync(id);
        if (labTest == null)
            return NotFound();

        return Ok(labTest);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLabTestDto createDto)
    {
        try
        {
            var patient = await _unitOfWork.Patients.GetByIdAsync(createDto.PatientId);
            if (patient == null)
                return NotFound("Patient not found");

            var doctor = await _unitOfWork.Doctors.GetByIdAsync(createDto.DoctorId);
            if (doctor == null)
                return NotFound("Doctor not found");

            var testType = await _unitOfWork.LabTestTypes.GetByIdAsync(createDto.LabTestTypeId);
            if (testType == null)
                return NotFound("Lab test type not found");

            var labTest = new LabTest
            {
                PatientId = createDto.PatientId,
                DoctorId = createDto.DoctorId,
                LabTestTypeId = createDto.LabTestTypeId,
                AppointmentId = createDto.AppointmentId,
                TestNumber = $"LAB-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}",
                Status = "Pending",
                Price = testType.Price,
                RequestedDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.LabTests.AddAsync(labTest);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = labTest.Id }, labTest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating lab test");
            return StatusCode(500, new { message = "An error occurred while creating the lab test" });
        }
    }

    [HttpPut("{id}/results")]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<IActionResult> UpdateResults(int id, [FromBody] UpdateLabTestResultsDto updateDto)
    {
        var labTest = await _unitOfWork.LabTests.GetByIdAsync(id);
        if (labTest == null)
            return NotFound();

        labTest.Results = updateDto.Results;
        labTest.Notes = updateDto.Notes;
        labTest.Status = "Completed";
        labTest.CompletedDate = DateTime.UtcNow;
        labTest.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.LabTests.UpdateAsync(labTest);
        await _unitOfWork.SaveChangesAsync();

        return Ok(labTest);
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateLabTestStatusDto updateDto)
    {
        var labTest = await _unitOfWork.LabTests.GetByIdAsync(id);
        if (labTest == null)
            return NotFound();

        labTest.Status = updateDto.Status;
        if (updateDto.Status == "Completed")
        {
            labTest.CompletedDate = DateTime.UtcNow;
        }
        labTest.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.LabTests.UpdateAsync(labTest);
        await _unitOfWork.SaveChangesAsync();

        return Ok(labTest);
    }
}

public class CreateLabTestDto
{
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public int LabTestTypeId { get; set; }
    public int? AppointmentId { get; set; }
}

public class UpdateLabTestResultsDto
{
    public string Results { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class UpdateLabTestStatusDto
{
    public string Status { get; set; } = string.Empty;
}

