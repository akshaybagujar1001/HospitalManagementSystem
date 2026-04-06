using HMS.Application.DTOs.Radiology;
using HMS.Domain.Entities;
using HMS.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HMS.API.Controllers;

[ApiController]
[Route("api/radiology-orders")]
[Authorize]
public class RadiologyOrdersController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RadiologyOrdersController> _logger;

    public RadiologyOrdersController(IUnitOfWork unitOfWork, ILogger<RadiologyOrdersController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Roles = "Doctor,Admin")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateRadiologyOrderDto createDto)
    {
        try
        {
            var order = new RadiologyOrder
            {
                PatientId = createDto.PatientId,
                DoctorId = createDto.DoctorId,
                Type = createDto.Type,
                Description = createDto.Description,
                Status = "Requested",
                RequestedAt = DateTime.UtcNow,
                Cost = createDto.Cost
            };

            await _unitOfWork.RadiologyOrders.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating radiology order");
            return StatusCode(500, new { message = "An error occurred while creating the radiology order" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? patientId, [FromQuery] int? doctorId, [FromQuery] string? status)
    {
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var orders = await _unitOfWork.RadiologyOrders.FindAsync(o =>
            (patientId == null || o.PatientId == patientId) &&
            (doctorId == null || o.DoctorId == doctorId) &&
            (status == null || o.Status == status) &&
            (userRole != "Patient" || o.Patient.UserId == userId));

        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var order = await _unitOfWork.RadiologyOrders.GetByIdAsync(id);
        if (order == null)
            return NotFound();

        return Ok(order);
    }

    [HttpGet("patient/{patientId}")]
    public async Task<IActionResult> GetPatientOrders(int patientId)
    {
        var orders = await _unitOfWork.RadiologyOrders.FindAsync(o => o.PatientId == patientId);
        return Ok(orders);
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Doctor,Admin")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateRadiologyOrderStatusDto updateDto)
    {
        var order = await _unitOfWork.RadiologyOrders.GetByIdAsync(id);
        if (order == null)
            return NotFound();

        order.Status = updateDto.Status;
        
        if (updateDto.Status == "Scheduled" && updateDto.ScheduledAt.HasValue)
            order.ScheduledAt = updateDto.ScheduledAt;
        
        if (updateDto.Status == "Completed")
        {
            order.CompletedAt = DateTime.UtcNow;
            if (updateDto.RadiologistId.HasValue)
                order.RadiologistId = updateDto.RadiologistId;
        }

        await _unitOfWork.RadiologyOrders.UpdateAsync(order);
        await _unitOfWork.SaveChangesAsync();

        return Ok(new { message = "Order status updated" });
    }
}

