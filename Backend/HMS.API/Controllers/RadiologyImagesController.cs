using HMS.Application.DTOs.Radiology;
using HMS.Domain.Entities;
using HMS.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HMS.API.Controllers;

[ApiController]
[Route("api/radiology-images")]
[Authorize]
public class RadiologyImagesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RadiologyImagesController> _logger;

    public RadiologyImagesController(IUnitOfWork unitOfWork, ILogger<RadiologyImagesController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Roles = "Doctor,Admin")]
    public async Task<IActionResult> UploadImage([FromBody] CreateRadiologyImageDto createDto)
    {
        try
        {
            var order = await _unitOfWork.RadiologyOrders.GetByIdAsync(createDto.RadiologyOrderId);
            if (order == null)
                return NotFound("Radiology order not found");

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var image = new RadiologyImage
            {
                RadiologyOrderId = createDto.RadiologyOrderId,
                FilePath = createDto.FilePath, // In production, handle actual file upload
                FileName = createDto.FileName,
                FileType = createDto.FileType,
                FileSize = createDto.FileSize,
                Description = createDto.Description,
                UploadedByUserId = userId,
                UploadedAt = DateTime.UtcNow
            };

            await _unitOfWork.RadiologyImages.AddAsync(image);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = image.Id }, image);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading radiology image");
            return StatusCode(500, new { message = "An error occurred while uploading the image" });
        }
    }

    [HttpGet("order/{orderId}")]
    public async Task<IActionResult> GetByOrder(int orderId)
    {
        var images = await _unitOfWork.RadiologyImages.FindAsync(i => i.RadiologyOrderId == orderId);
        return Ok(images);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var image = await _unitOfWork.RadiologyImages.GetByIdAsync(id);
        if (image == null)
            return NotFound();

        return Ok(image);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var image = await _unitOfWork.RadiologyImages.GetByIdAsync(id);
        if (image == null)
            return NotFound();

        // In production, also delete the actual file from storage
        await _unitOfWork.RadiologyImages.DeleteAsync(image);
        await _unitOfWork.SaveChangesAsync();

        return Ok(new { message = "Image deleted" });
    }
}

