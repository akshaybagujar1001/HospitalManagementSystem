using HMS.Application.DTOs.Insurance;
using HMS.Domain.Entities;
using HMS.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HMS.API.Controllers;

[ApiController]
[Route("api/insurance-policies")]
[Authorize]
public class InsurancePoliciesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<InsurancePoliciesController> _logger;

    public InsurancePoliciesController(IUnitOfWork unitOfWork, ILogger<InsurancePoliciesController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet("patient/{patientId}")]
    public async Task<IActionResult> GetPatientPolicies(int patientId, [FromQuery] bool? activeOnly = false)
    {
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        // Patients can only see their own policies
        if (userRole == "Patient")
        {
            var patient = await _unitOfWork.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient == null || patient.Id != patientId)
                return Forbid();
        }

        var policies = await _unitOfWork.InsurancePolicies.FindAsync(p =>
            p.PatientId == patientId &&
            (!activeOnly.HasValue || !activeOnly.Value || p.IsActive));

        var policyDtos = policies.Select(p => new InsurancePolicyDto
        {
            Id = p.Id,
            PatientId = p.PatientId,
            CompanyId = p.CompanyId,
            PolicyNumber = p.PolicyNumber,
            CoveragePercentage = p.CoveragePercentage,
            ExpirationDate = p.ExpirationDate,
            IsActive = p.IsActive,
            CreatedAt = p.CreatedAt
        });

        return Ok(policyDtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var policy = await _unitOfWork.InsurancePolicies.GetByIdAsync(id);
        if (policy == null)
            return NotFound();

        return Ok(policy);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Finance")]
    public async Task<IActionResult> Create([FromBody] CreateInsurancePolicyDto createDto)
    {
        try
        {
            // Validate patient exists
            var patient = await _unitOfWork.Patients.GetByIdAsync(createDto.PatientId);
            if (patient == null)
                return NotFound("Patient not found");

            // Validate company exists
            var company = await _unitOfWork.InsuranceCompanies.GetByIdAsync(createDto.CompanyId);
            if (company == null)
                return NotFound("Insurance company not found");

            var policy = new InsurancePolicy
            {
                PatientId = createDto.PatientId,
                CompanyId = createDto.CompanyId,
                PolicyNumber = createDto.PolicyNumber,
                CoveragePercentage = createDto.CoveragePercentage,
                ExpirationDate = createDto.ExpirationDate,
                IsActive = true,
                Notes = createDto.Notes,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.InsurancePolicies.AddAsync(policy);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = policy.Id }, policy);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating insurance policy");
            return StatusCode(500, new { message = "An error occurred while creating the insurance policy" });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Finance")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateInsurancePolicyDto updateDto)
    {
        var policy = await _unitOfWork.InsurancePolicies.GetByIdAsync(id);
        if (policy == null)
            return NotFound();

        policy.CoveragePercentage = updateDto.CoveragePercentage;
        policy.ExpirationDate = updateDto.ExpirationDate;
        policy.IsActive = updateDto.IsActive;
        policy.Notes = updateDto.Notes;
        policy.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.InsurancePolicies.UpdateAsync(policy);
        await _unitOfWork.SaveChangesAsync();

        return Ok(policy);
    }
}

