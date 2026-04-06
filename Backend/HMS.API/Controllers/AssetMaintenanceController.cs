using HMS.Application.DTOs.Asset;
using HMS.Domain.Entities;
using HMS.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HMS.API.Controllers;

[ApiController]
[Route("api/asset-maintenance")]
[Authorize(Roles = "Admin")]
public class AssetMaintenanceController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AssetMaintenanceController> _logger;

    public AssetMaintenanceController(IUnitOfWork unitOfWork, ILogger<AssetMaintenanceController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateMaintenance([FromBody] CreateAssetMaintenanceDto createDto)
    {
        try
        {
            var asset = await _unitOfWork.Assets.GetByIdAsync(createDto.AssetId);
            if (asset == null)
                return NotFound("Asset not found");

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var maintenance = new AssetMaintenance
            {
                AssetId = createDto.AssetId,
                Description = createDto.Description,
                MaintenanceType = createDto.MaintenanceType,
                Cost = createDto.Cost,
                PerformedAt = createDto.PerformedAt ?? DateTime.UtcNow,
                PerformedByUserId = userId,
                Vendor = createDto.Vendor,
                Notes = createDto.Notes
            };

            // Update asset maintenance dates
            asset.LastMaintenanceDate = maintenance.PerformedAt;
            if (createDto.NextMaintenanceDate.HasValue)
                asset.NextMaintenanceDate = createDto.NextMaintenanceDate;

            await _unitOfWork.AssetMaintenances.AddAsync(maintenance);
            await _unitOfWork.Assets.UpdateAsync(asset);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = maintenance.Id }, maintenance);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating asset maintenance");
            return StatusCode(500, new { message = "An error occurred while creating the maintenance record" });
        }
    }

    [HttpGet("asset/{assetId}")]
    public async Task<IActionResult> GetByAsset(int assetId)
    {
        var maintenances = await _unitOfWork.AssetMaintenances.FindAsync(m => m.AssetId == assetId);
        return Ok(maintenances.OrderByDescending(m => m.PerformedAt));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var maintenance = await _unitOfWork.AssetMaintenances.GetByIdAsync(id);
        if (maintenance == null)
            return NotFound();

        return Ok(maintenance);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? assetId)
    {
        var maintenances = await _unitOfWork.AssetMaintenances.FindAsync(m =>
            assetId == null || m.AssetId == assetId);

        return Ok(maintenances.OrderByDescending(m => m.PerformedAt));
    }
}

