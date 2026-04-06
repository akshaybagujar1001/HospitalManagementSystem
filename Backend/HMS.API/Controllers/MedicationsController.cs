using HMS.Application.DTOs.Pharmacy;
using HMS.Domain.Entities;
using HMS.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace HMS.API.Controllers;

[ApiController]
[Route("api/medications")]
[Authorize(Roles = "Admin,Pharmacist")]
public class MedicationsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<MedicationsController> _logger;

    public MedicationsController(IUnitOfWork unitOfWork, ILogger<MedicationsController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool? lowStockOnly = false)
    {
        try
        {
            var medications = await _unitOfWork.Medications.FindAsync(m =>
                !lowStockOnly.HasValue || !lowStockOnly.Value || m.StockQuantity <= m.ReorderLevel);

            var medicationDtos = medications.Select(m => new MedicationDto
            {
                Id = m.Id,
                Name = m.Name,
                Code = m.Code,
                DosageForm = m.DosageForm,
                Strength = m.Strength,
                StockQuantity = m.StockQuantity,
                ReorderLevel = m.ReorderLevel,
                Price = m.Price,
                ExpirationDate = m.ExpirationDate,
                Manufacturer = m.Manufacturer,
                IsActive = m.IsActive
            });

            return Ok(medicationDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting medications: {Error}", ex.Message);
            return StatusCode(500, new { message = "An error occurred while retrieving medications", details = ex.Message });
        }
    }

    [HttpGet("low-stock")]
    public async Task<IActionResult> GetLowStock()
    {
        try
        {
            var medications = await _unitOfWork.Medications.FindAsync(m =>
                m.StockQuantity <= m.ReorderLevel && m.IsActive);

            var medicationDtos = medications.Select(m => new MedicationDto
            {
                Id = m.Id,
                Name = m.Name,
                Code = m.Code,
                DosageForm = m.DosageForm,
                Strength = m.Strength,
                StockQuantity = m.StockQuantity,
                ReorderLevel = m.ReorderLevel,
                Price = m.Price,
                ExpirationDate = m.ExpirationDate,
                Manufacturer = m.Manufacturer,
                IsActive = m.IsActive
            });

            return Ok(medicationDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting low stock medications: {Error}", ex.Message);
            return StatusCode(500, new { message = "An error occurred while retrieving low stock medications", details = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var medication = await _unitOfWork.Medications.GetByIdAsync(id);
        if (medication == null)
            return NotFound();

        return Ok(medication);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateMedicationDto createDto)
    {
        try
        {
            if (createDto == null)
            {
                return BadRequest(new { message = "Request body is required" });
            }

            if (string.IsNullOrWhiteSpace(createDto.Name))
            {
                return BadRequest(new { message = "Medication name is required" });
            }

            if (createDto.Price < 0)
            {
                return BadRequest(new { message = "Price cannot be negative" });
            }

            var medication = new Medication
            {
                Name = createDto.Name.Trim(),
                Code = createDto.Code?.Trim(),
                DosageForm = createDto.DosageForm?.Trim(),
                Strength = createDto.Strength?.Trim(),
                StockQuantity = createDto.StockQuantity ?? 0,
                ReorderLevel = createDto.ReorderLevel,
                Price = createDto.Price,
                ExpirationDate = createDto.ExpirationDate,
                Manufacturer = createDto.Manufacturer?.Trim(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Medications.AddAsync(medication);
            await _unitOfWork.SaveChangesAsync();

            var medicationDto = new MedicationDto
            {
                Id = medication.Id,
                Name = medication.Name,
                Code = medication.Code,
                DosageForm = medication.DosageForm,
                Strength = medication.Strength,
                StockQuantity = medication.StockQuantity,
                ReorderLevel = medication.ReorderLevel,
                Price = medication.Price,
                ExpirationDate = medication.ExpirationDate,
                Manufacturer = medication.Manufacturer,
                IsActive = medication.IsActive
            };

            return CreatedAtAction(nameof(GetById), new { id = medication.Id }, medicationDto);
        }
        catch (DbUpdateException dbEx)
        {
            // Log FULL exception details
            _logger.LogError(dbEx, "=== DATABASE ERROR CREATING MEDICATION ===");
            _logger.LogError("Exception Type: {Type}", dbEx.GetType().FullName);
            _logger.LogError("Exception Message: {Message}", dbEx.Message);
            _logger.LogError("Stack Trace: {StackTrace}", dbEx.StackTrace);
            
            // Log inner exception details
            var innerEx = dbEx.InnerException;
            int depth = 0;
            while (innerEx != null)
            {
                depth++;
                _logger.LogError("Inner Exception [{Depth}]: {Type} - {Message}", depth, innerEx.GetType().FullName, innerEx.Message);
                _logger.LogError("Inner Exception [{Depth}] Stack Trace: {StackTrace}", depth, innerEx.StackTrace);
                
                // Check for SqlException
                if (innerEx is Microsoft.Data.SqlClient.SqlException sqlEx)
                {
                    _logger.LogError("SQL Exception Number: {Number}", sqlEx.Number);
                    _logger.LogError("SQL Exception State: {State}", sqlEx.State);
                    _logger.LogError("SQL Exception Class: {Class}", sqlEx.Class);
                    _logger.LogError("SQL Exception Server: {Server}", sqlEx.Server);
                    _logger.LogError("SQL Exception Procedure: {Procedure}", sqlEx.Procedure);
                    _logger.LogError("SQL Exception LineNumber: {LineNumber}", sqlEx.LineNumber);
                    _logger.LogError("SQL Errors Count: {Count}", sqlEx.Errors.Count);
                    for (int i = 0; i < sqlEx.Errors.Count; i++)
                    {
                        var error = sqlEx.Errors[i];
                        _logger.LogError("SQL Error [{Index}]: Number={Number}, State={State}, Class={Class}, Message={Message}, Source={Source}", 
                            i, error.Number, error.State, error.Class, error.Message, error.Source);
                    }
                }
                
                innerEx = innerEx.InnerException;
            }
            
            // Log entity state if available
            if (dbEx.Entries != null && dbEx.Entries.Any())
            {
                foreach (var entry in dbEx.Entries)
                {
                    _logger.LogError("Entity: {EntityType}, State: {State}", entry.Entity.GetType().Name, entry.State);
                    foreach (var prop in entry.Properties)
                    {
                        _logger.LogError("  Property: {Property} = {Value} (IsModified: {IsModified}, IsTemporary: {IsTemporary})", 
                            prop.Metadata.Name, prop.CurrentValue, prop.IsModified, prop.IsTemporary);
                    }
                }
            }
            
            _logger.LogError("=== END DATABASE ERROR ===");
            
            // Return detailed error
            var errorDetails = new
            {
                message = "Database error occurred",
                exceptionType = dbEx.GetType().FullName,
                exceptionMessage = dbEx.Message,
                stackTrace = dbEx.StackTrace,
                innerException = dbEx.InnerException?.Message,
                sqlException = dbEx.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx2 
                    ? new
                    {
                        number = sqlEx2.Number,
                        state = sqlEx2.State,
                        class_ = sqlEx2.Class,
                        message = sqlEx2.Message,
                        server = sqlEx2.Server,
                        errors = sqlEx2.Errors.Cast<Microsoft.Data.SqlClient.SqlError>()
                            .Select(e => new { number = e.Number, state = e.State, message = e.Message, source = e.Source })
                            .ToList()
                    }
                    : null
            };
            
            return StatusCode(500, errorDetails);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "=== GENERAL ERROR CREATING MEDICATION ===");
            _logger.LogError("Exception Type: {Type}", ex.GetType().FullName);
            _logger.LogError("Exception Message: {Message}", ex.Message);
            _logger.LogError("Stack Trace: {StackTrace}", ex.StackTrace);
            
            var innerEx = ex.InnerException;
            int depth = 0;
            while (innerEx != null)
            {
                depth++;
                _logger.LogError("Inner Exception [{Depth}]: {Type} - {Message}", depth, innerEx.GetType().FullName, innerEx.Message);
                _logger.LogError("Inner Exception [{Depth}] Stack Trace: {StackTrace}", depth, innerEx.StackTrace);
                innerEx = innerEx.InnerException;
            }
            _logger.LogError("=== END GENERAL ERROR ===");
            
            return StatusCode(500, new 
            { 
                message = "An error occurred while creating the medication",
                exceptionType = ex.GetType().FullName,
                exceptionMessage = ex.Message,
                stackTrace = ex.StackTrace,
                innerException = ex.InnerException?.Message
            });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateMedicationDto updateDto)
    {
        var medication = await _unitOfWork.Medications.GetByIdAsync(id);
        if (medication == null)
            return NotFound();

        medication.Name = updateDto.Name;
        medication.DosageForm = updateDto.DosageForm;
        medication.Strength = updateDto.Strength;
        medication.ReorderLevel = updateDto.ReorderLevel;
        medication.Price = updateDto.Price;
        medication.ExpirationDate = updateDto.ExpirationDate;
        medication.Manufacturer = updateDto.Manufacturer;
        medication.IsActive = updateDto.IsActive;
        medication.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Medications.UpdateAsync(medication);
        await _unitOfWork.SaveChangesAsync();

        return Ok(medication);
    }

    [HttpPost("{id}/adjust-stock")]
    [Authorize(Roles = "Admin,Pharmacist")]
    public async Task<IActionResult> AdjustStock(int id, [FromBody] AdjustStockDto adjustDto)
    {
        var medication = await _unitOfWork.Medications.GetByIdAsync(id);
        if (medication == null)
            return NotFound();

        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

        // Create stock movement
        var movement = new MedicationStockMovement
        {
            MedicationId = id,
            QuantityChange = adjustDto.QuantityChange,
            MovementType = adjustDto.QuantityChange > 0 ? "In" : "Out",
            Reason = adjustDto.Reason,
            PerformedByUserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        // Update stock
        medication.StockQuantity += adjustDto.QuantityChange;
        medication.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.MedicationStockMovements.AddAsync(movement);
        await _unitOfWork.Medications.UpdateAsync(medication);
        await _unitOfWork.SaveChangesAsync();

        return Ok(new { message = "Stock adjusted", newQuantity = medication.StockQuantity });
    }
}

