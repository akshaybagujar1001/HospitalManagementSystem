using HMS.Application.DTOs.Finance;
using HMS.Domain.Entities;
using HMS.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HMS.API.Controllers;

[ApiController]
[Route("api/revenues")]
[Authorize(Roles = "Admin,Finance")]
public class RevenuesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RevenuesController> _logger;

    public RevenuesController(IUnitOfWork unitOfWork, ILogger<RevenuesController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? sourceType, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var revenues = await _unitOfWork.RevenueRecords.FindAsync(r =>
            (sourceType == null || r.SourceType == sourceType) &&
            (!startDate.HasValue || r.RevenueDate >= startDate.Value) &&
            (!endDate.HasValue || r.RevenueDate <= endDate.Value));

        var revenueDtos = revenues.Select(r => new RevenueRecordDto
        {
            Id = r.Id,
            SourceType = r.SourceType,
            Amount = r.Amount,
            InvoiceId = r.InvoiceId,
            PatientId = r.PatientId,
            Description = r.Description,
            PaymentMethod = r.PaymentMethod,
            RevenueDate = r.RevenueDate,
            CreatedAt = r.CreatedAt
        });

        return Ok(revenueDtos);
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var start = startDate ?? DateTime.UtcNow.AddMonths(-1);
        var end = endDate ?? DateTime.UtcNow;

        var revenues = await _unitOfWork.RevenueRecords.FindAsync(r =>
            r.RevenueDate >= start && r.RevenueDate <= end);

        var total = revenues.Sum(r => r.Amount);
        var bySource = revenues.GroupBy(r => r.SourceType)
            .Select(g => new { SourceType = g.Key, Total = g.Sum(r => r.Amount) });

        return Ok(new
        {
            Total = total,
            BySource = bySource,
            PeriodStart = start,
            PeriodEnd = end
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var revenue = await _unitOfWork.RevenueRecords.GetByIdAsync(id);
        if (revenue == null)
            return NotFound();

        return Ok(revenue);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRevenueRecordDto createDto)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var revenue = new RevenueRecord
            {
                SourceType = createDto.SourceType,
                Amount = createDto.Amount,
                InvoiceId = createDto.InvoiceId,
                PatientId = createDto.PatientId,
                Description = createDto.Description,
                PaymentMethod = createDto.PaymentMethod,
                RevenueDate = createDto.RevenueDate ?? DateTime.UtcNow,
                CreatedByUserId = userId,
                Notes = createDto.Notes,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.RevenueRecords.AddAsync(revenue);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = revenue.Id }, revenue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating revenue record");
            return StatusCode(500, new { message = "An error occurred while creating the revenue record" });
        }
    }
}

