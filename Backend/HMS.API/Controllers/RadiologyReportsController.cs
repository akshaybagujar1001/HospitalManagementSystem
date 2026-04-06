using HMS.Application.DTOs.Radiology;
using HMS.Domain.Entities;
using HMS.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.API.Controllers;

[ApiController]
[Route("api/radiology-reports")]
[Authorize(Roles = "Doctor,Admin")]
public class RadiologyReportsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RadiologyReportsController> _logger;

    public RadiologyReportsController(IUnitOfWork unitOfWork, ILogger<RadiologyReportsController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateReport([FromBody] CreateRadiologyReportDto createDto)
    {
        try
        {
            var order = await _unitOfWork.RadiologyOrders.GetByIdAsync(createDto.RadiologyOrderId);
            if (order == null)
                return NotFound("Radiology order not found");

            // Check if report already exists
            var existingReport = await _unitOfWork.RadiologyReports.FirstOrDefaultAsync(r => r.RadiologyOrderId == createDto.RadiologyOrderId);
            if (existingReport != null)
                return BadRequest("Report already exists for this order");

            var report = new RadiologyReport
            {
                RadiologyOrderId = createDto.RadiologyOrderId,
                RadiologistId = createDto.RadiologistId,
                ReportText = createDto.ReportText,
                Findings = createDto.Findings,
                Impression = createDto.Impression,
                Status = createDto.Status ?? "Draft",
                CreatedAt = DateTime.UtcNow
            };

            if (report.Status == "Final")
                report.FinalizedAt = DateTime.UtcNow;

            await _unitOfWork.RadiologyReports.AddAsync(report);
            
            // Update order status
            order.Status = "Completed";
            order.CompletedAt = DateTime.UtcNow;
            order.RadiologistId = createDto.RadiologistId;
            await _unitOfWork.RadiologyOrders.UpdateAsync(order);
            
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(nameof(GetByOrder), new { orderId = report.RadiologyOrderId }, report);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating radiology report");
            return StatusCode(500, new { message = "An error occurred while creating the radiology report" });
        }
    }

    [HttpGet("order/{orderId}")]
    public async Task<IActionResult> GetByOrder(int orderId)
    {
        var report = await _unitOfWork.RadiologyReports.FirstOrDefaultAsync(r => r.RadiologyOrderId == orderId);
        if (report == null)
            return NotFound();

        return Ok(report);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReport(int id, [FromBody] UpdateRadiologyReportDto updateDto)
    {
        var report = await _unitOfWork.RadiologyReports.GetByIdAsync(id);
        if (report == null)
            return NotFound();

        report.ReportText = updateDto.ReportText;
        report.Findings = updateDto.Findings;
        report.Impression = updateDto.Impression;
        report.Status = updateDto.Status;

        if (updateDto.Status == "Final" && report.FinalizedAt == null)
            report.FinalizedAt = DateTime.UtcNow;

        await _unitOfWork.RadiologyReports.UpdateAsync(report);
        await _unitOfWork.SaveChangesAsync();

        return Ok(report);
    }
}

