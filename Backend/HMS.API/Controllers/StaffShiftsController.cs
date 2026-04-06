using HMS.Application.DTOs.Shift;
using HMS.Domain.Entities;
using HMS.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HMS.API.Controllers;

[ApiController]
[Route("api/staff-shifts")]
[Authorize(Roles = "Admin")]
public class StaffShiftsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<StaffShiftsController> _logger;

    public StaffShiftsController(IUnitOfWork unitOfWork, ILogger<StaffShiftsController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? staffId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var shifts = await _unitOfWork.StaffShifts.FindAsync(s =>
            (staffId == null || s.StaffId == staffId) &&
            (!startDate.HasValue || s.StartTime >= startDate.Value) &&
            (!endDate.HasValue || s.EndTime <= endDate.Value));

        var shiftDtos = shifts.Select(s => new StaffShiftDto
        {
            Id = s.Id,
            StaffId = s.StaffId,
            Role = s.Role,
            StartTime = s.StartTime,
            EndTime = s.EndTime,
            Department = s.Department,
            IsNightShift = s.IsNightShift
        });

        return Ok(shiftDtos);
    }

    [HttpGet("staff/{staffId}")]
    public async Task<IActionResult> GetStaffShifts(int staffId)
    {
        var shifts = await _unitOfWork.StaffShifts.FindAsync(s => s.StaffId == staffId);
        return Ok(shifts.OrderBy(s => s.StartTime));
    }

    [HttpGet("week")]
    public async Task<IActionResult> GetWeekShifts([FromQuery] DateTime startDate)
    {
        var endDate = startDate.AddDays(7);
        var shifts = await _unitOfWork.StaffShifts.FindAsync(s =>
            s.StartTime >= startDate && s.StartTime < endDate);

        return Ok(shifts.OrderBy(s => s.StartTime).ThenBy(s => s.Department));
    }

    [HttpPost]
    public async Task<IActionResult> CreateShift([FromBody] CreateStaffShiftDto createDto)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            // Check for overlapping shifts
            var overlapping = await _unitOfWork.StaffShifts.FindAsync(s =>
                s.StaffId == createDto.StaffId &&
                ((s.StartTime <= createDto.StartTime && s.EndTime > createDto.StartTime) ||
                 (s.StartTime < createDto.EndTime && s.EndTime >= createDto.EndTime) ||
                 (s.StartTime >= createDto.StartTime && s.EndTime <= createDto.EndTime)));

            if (overlapping.Any())
                return BadRequest(new { message = "Shift overlaps with existing shift" });

            var shift = new StaffShift
            {
                StaffId = createDto.StaffId,
                Role = createDto.Role,
                StartTime = createDto.StartTime,
                EndTime = createDto.EndTime,
                Department = createDto.Department,
                IsNightShift = createDto.StartTime.Hour >= 20 || createDto.StartTime.Hour < 6,
                Notes = createDto.Notes,
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.StaffShifts.AddAsync(shift);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = shift.Id }, shift);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating staff shift");
            return StatusCode(500, new { message = "An error occurred while creating the shift" });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var shift = await _unitOfWork.StaffShifts.GetByIdAsync(id);
        if (shift == null)
            return NotFound();

        return Ok(shift);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateShift(int id, [FromBody] UpdateStaffShiftDto updateDto)
    {
        var shift = await _unitOfWork.StaffShifts.GetByIdAsync(id);
        if (shift == null)
            return NotFound();

        shift.StartTime = updateDto.StartTime;
        shift.EndTime = updateDto.EndTime;
        shift.Department = updateDto.Department;
        shift.IsNightShift = updateDto.StartTime.Hour >= 20 || updateDto.StartTime.Hour < 6;
        shift.Notes = updateDto.Notes;

        await _unitOfWork.StaffShifts.UpdateAsync(shift);
        await _unitOfWork.SaveChangesAsync();

        return Ok(shift);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteShift(int id)
    {
        var shift = await _unitOfWork.StaffShifts.GetByIdAsync(id);
        if (shift == null)
            return NotFound();

        await _unitOfWork.StaffShifts.DeleteAsync(shift);
        await _unitOfWork.SaveChangesAsync();

        return Ok(new { message = "Shift deleted" });
    }
}

