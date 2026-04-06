using HMS.Application.DTOs.Insurance;
using HMS.Domain.Entities;
using HMS.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HMS.API.Controllers;

[ApiController]
[Route("api/insurance-claims")]
[Authorize(Roles = "Admin,Finance")]
public class InsuranceClaimsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly HMS.Infrastructure.Data.HmsDbContext _context;
    private readonly ILogger<InsuranceClaimsController> _logger;

    public InsuranceClaimsController(
        IUnitOfWork unitOfWork,
        HMS.Infrastructure.Data.HmsDbContext context,
        ILogger<InsuranceClaimsController> logger)
    {
        _unitOfWork = unitOfWork;
        _context = context;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateClaim([FromBody] CreateInsuranceClaimDto createDto)
    {
        try
        {
            var invoice = await _unitOfWork.Invoices.GetByIdAsync(createDto.InvoiceId);
            if (invoice == null)
                return NotFound("Invoice not found");

            var policy = await _unitOfWork.InsurancePolicies.GetByIdAsync(createDto.PolicyId);
            if (policy == null || !policy.IsActive)
                return BadRequest("Invalid or inactive insurance policy");

            // Check if policy is expired
            if (policy.ExpirationDate.HasValue && policy.ExpirationDate.Value < DateTime.UtcNow)
                return BadRequest("Insurance policy has expired");

            // Calculate coverage
            var totalAmount = invoice.FinalAmount;
            var amountCovered = totalAmount * (policy.CoveragePercentage / 100);
            var patientResponsibility = totalAmount - amountCovered;

            var claim = new InsuranceClaim
            {
                PatientId = invoice.PatientId,
                InvoiceId = invoice.Id,
                CompanyId = policy.CompanyId,
                PolicyId = policy.Id,
                Status = "Pending",
                TotalAmount = totalAmount,
                AmountCovered = amountCovered,
                PatientResponsibility = patientResponsibility,
                SubmittedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.InsuranceClaims.AddAsync(claim);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = claim.Id }, claim);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating insurance claim");
            return StatusCode(500, new { message = "An error occurred while creating the insurance claim" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? status, [FromQuery] int? patientId)
    {
        var claims = await _unitOfWork.InsuranceClaims.FindAsync(c =>
            (status == null || c.Status == status) &&
            (patientId == null || c.PatientId == patientId));

        var claimDtos = claims.Select(c => new InsuranceClaimDto
        {
            Id = c.Id,
            PatientId = c.PatientId,
            InvoiceId = c.InvoiceId,
            CompanyId = c.CompanyId,
            PolicyId = c.PolicyId,
            Status = c.Status,
            TotalAmount = c.TotalAmount,
            AmountCovered = c.AmountCovered,
            PatientResponsibility = c.PatientResponsibility,
            SubmittedAt = c.SubmittedAt,
            ProcessedAt = c.ProcessedAt,
            CreatedAt = c.CreatedAt
        });

        return Ok(claimDtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var claim = await _context.InsuranceClaims
            .Include(c => c.Patient)
            .Include(c => c.Company)
            .Include(c => c.Policy)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (claim == null)
            return NotFound();

        return Ok(claim);
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateClaimStatusDto updateDto)
    {
        var claim = await _unitOfWork.InsuranceClaims.GetByIdAsync(id);
        if (claim == null)
            return NotFound();

        claim.Status = updateDto.Status;
        claim.ProcessedAt = DateTime.UtcNow;
        claim.Notes = updateDto.Notes;
        claim.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.InsuranceClaims.UpdateAsync(claim);
        await _unitOfWork.SaveChangesAsync();

        return Ok(new { message = "Claim status updated" });
    }

    [HttpGet("patient/{patientId}")]
    public async Task<IActionResult> GetPatientClaims(int patientId)
    {
        var claims = await _unitOfWork.InsuranceClaims.FindAsync(c => c.PatientId == patientId);
        return Ok(claims);
    }
}

