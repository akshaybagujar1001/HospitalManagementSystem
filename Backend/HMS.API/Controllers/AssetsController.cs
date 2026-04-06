using HMS.Application.DTOs.Asset;
using HMS.Domain.Entities;
using HMS.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.API.Controllers;

[ApiController]
[Route("api/assets")]
[Authorize(Roles = "Admin")]
public class AssetsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AssetsController> _logger;

    public AssetsController(IUnitOfWork unitOfWork, ILogger<AssetsController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? type, [FromQuery] string? status)
    {
        var assets = await _unitOfWork.Assets.FindAsync(a =>
            (type == null || a.Type == type) &&
            (status == null || a.Status == status));

        var assetDtos = assets.Select(a => new AssetDto
        {
            Id = a.Id,
            Name = a.Name,
            Type = a.Type,
            SerialNumber = a.SerialNumber,
            Manufacturer = a.Manufacturer,
            Model = a.Model,
            Location = a.Location,
            Status = a.Status,
            PurchasePrice = a.PurchasePrice,
            PurchaseDate = a.PurchaseDate,
            LastMaintenanceDate = a.LastMaintenanceDate,
            NextMaintenanceDate = a.NextMaintenanceDate
        });

        return Ok(assetDtos);
    }

    [HttpGet("maintenance-due")]
    public async Task<IActionResult> GetMaintenanceDue()
    {
        var assets = await _unitOfWork.Assets.FindAsync(a =>
            a.NextMaintenanceDate.HasValue &&
            a.NextMaintenanceDate.Value <= DateTime.UtcNow.AddDays(7));

        return Ok(assets);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var asset = await _unitOfWork.Assets.GetByIdAsync(id);
        if (asset == null)
            return NotFound();

        var assetDto = new AssetDto
        {
            Id = asset.Id,
            Name = asset.Name,
            Type = asset.Type,
            SerialNumber = asset.SerialNumber,
            Manufacturer = asset.Manufacturer,
            Model = asset.Model,
            Location = asset.Location,
            Status = asset.Status,
            PurchasePrice = asset.PurchasePrice,
            PurchaseDate = asset.PurchaseDate,
            LastMaintenanceDate = asset.LastMaintenanceDate,
            NextMaintenanceDate = asset.NextMaintenanceDate
        };

        return Ok(assetDto);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAssetDto createDto)
    {
        try
        {
            var asset = new Asset
            {
                Name = createDto.Name,
                Type = createDto.Type,
                SerialNumber = createDto.SerialNumber,
                Manufacturer = createDto.Manufacturer,
                Model = createDto.Model,
                Location = createDto.Location,
                Status = createDto.Status ?? "Available",
                PurchasePrice = createDto.PurchasePrice,
                PurchaseDate = createDto.PurchaseDate,
                LastMaintenanceDate = createDto.LastMaintenanceDate,
                NextMaintenanceDate = createDto.NextMaintenanceDate,
                Notes = createDto.Notes,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Assets.AddAsync(asset);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = asset.Id }, asset);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating asset");
            return StatusCode(500, new { message = "An error occurred while creating the asset" });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAssetDto updateDto)
    {
        var asset = await _unitOfWork.Assets.GetByIdAsync(id);
        if (asset == null)
            return NotFound();

        asset.Name = updateDto.Name;
        asset.Type = updateDto.Type;
        asset.SerialNumber = updateDto.SerialNumber;
        asset.Manufacturer = updateDto.Manufacturer;
        asset.Model = updateDto.Model;
        asset.Location = updateDto.Location;
        asset.Status = updateDto.Status;
        asset.PurchasePrice = updateDto.PurchasePrice;
        asset.PurchaseDate = updateDto.PurchaseDate;
        asset.LastMaintenanceDate = updateDto.LastMaintenanceDate;
        asset.NextMaintenanceDate = updateDto.NextMaintenanceDate;
        asset.Notes = updateDto.Notes;
        asset.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Assets.UpdateAsync(asset);
        await _unitOfWork.SaveChangesAsync();

        return Ok(asset);
    }
}

