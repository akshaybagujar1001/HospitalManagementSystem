using HMS.Application.DTOs.Pharmacy;
using HMS.Domain.Entities;
using HMS.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HMS.API.Controllers;

[ApiController]
[Route("api/prescription-fulfillments")]
[Authorize(Roles = "Admin,Pharmacist")]
public class PrescriptionFulfillmentsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PrescriptionFulfillmentsController> _logger;

    public PrescriptionFulfillmentsController(IUnitOfWork unitOfWork, ILogger<PrescriptionFulfillmentsController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> FulfillPrescription([FromBody] CreatePrescriptionFulfillmentDto createDto)
    {
        try
        {
            var prescription = await _unitOfWork.Prescriptions.GetByIdAsync(createDto.PrescriptionId);
            if (prescription == null)
                return NotFound("Prescription not found");

            var medication = await _unitOfWork.Medications.GetByIdAsync(createDto.MedicationId);
            if (medication == null)
                return NotFound("Medication not found");

            // Check stock availability
            if (medication.StockQuantity < createDto.QuantityDispensed)
                return BadRequest(new { message = "Insufficient stock" });

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            // Create fulfillment
            var fulfillment = new PrescriptionFulfillment
            {
                PrescriptionId = createDto.PrescriptionId,
                MedicationId = createDto.MedicationId,
                QuantityDispensed = createDto.QuantityDispensed,
                DispensedByUserId = userId,
                DispensedAt = DateTime.UtcNow,
                Notes = createDto.Notes
            };

            // Update medication stock
            medication.StockQuantity -= createDto.QuantityDispensed;
            medication.UpdatedAt = DateTime.UtcNow;

            // Create stock movement
            var movement = new MedicationStockMovement
            {
                MedicationId = createDto.MedicationId,
                QuantityChange = -createDto.QuantityDispensed,
                MovementType = "Out",
                Reason = $"Prescription fulfillment - Prescription #{createDto.PrescriptionId}",
                PerformedByUserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.PrescriptionFulfillments.AddAsync(fulfillment);
            await _unitOfWork.Medications.UpdateAsync(medication);
            await _unitOfWork.MedicationStockMovements.AddAsync(movement);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = fulfillment.Id }, fulfillment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fulfilling prescription");
            return StatusCode(500, new { message = "An error occurred while fulfilling the prescription" });
        }
    }

    [HttpGet("prescription/{prescriptionId}")]
    public async Task<IActionResult> GetByPrescription(int prescriptionId)
    {
        var fulfillments = await _unitOfWork.PrescriptionFulfillments.FindAsync(f => f.PrescriptionId == prescriptionId);
        return Ok(fulfillments);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var fulfillment = await _unitOfWork.PrescriptionFulfillments.GetByIdAsync(id);
        if (fulfillment == null)
            return NotFound();

        return Ok(fulfillment);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var fulfillments = await _unitOfWork.PrescriptionFulfillments.FindAsync(f =>
            (!startDate.HasValue || f.DispensedAt >= startDate.Value) &&
            (!endDate.HasValue || f.DispensedAt <= endDate.Value));

        return Ok(fulfillments);
    }
}

