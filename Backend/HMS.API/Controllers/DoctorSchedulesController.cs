using HMS.Domain.Entities;
using HMS.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DoctorSchedulesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DoctorSchedulesController> _logger;

    public DoctorSchedulesController(IUnitOfWork unitOfWork, ILogger<DoctorSchedulesController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? doctorId)
    {
        IEnumerable<DoctorSchedule> schedules;

        if (doctorId.HasValue)
        {
            schedules = await _unitOfWork.DoctorSchedules.FindAsync(s => s.DoctorId == doctorId.Value && s.IsAvailable);
        }
        else
        {
            schedules = await _unitOfWork.DoctorSchedules.FindAsync(s => s.IsAvailable);
        }

        return Ok(schedules);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var schedule = await _unitOfWork.DoctorSchedules.GetByIdAsync(id);
        if (schedule == null)
            return NotFound();

        return Ok(schedule);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<IActionResult> Create([FromBody] CreateDoctorScheduleDto createDto)
    {
        try
        {
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(createDto.DoctorId);
            if (doctor == null)
                return NotFound("Doctor not found");

            var schedule = new DoctorSchedule
            {
                DoctorId = createDto.DoctorId,
                DayOfWeek = createDto.DayOfWeek,
                StartTime = createDto.StartTime,
                EndTime = createDto.EndTime,
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.DoctorSchedules.AddAsync(schedule);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = schedule.Id }, schedule);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating doctor schedule");
            return StatusCode(500, new { message = "An error occurred while creating the schedule" });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateDoctorScheduleDto updateDto)
    {
        var schedule = await _unitOfWork.DoctorSchedules.GetByIdAsync(id);
        if (schedule == null)
            return NotFound();

        schedule.DayOfWeek = updateDto.DayOfWeek;
        schedule.StartTime = updateDto.StartTime;
        schedule.EndTime = updateDto.EndTime;
        schedule.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.DoctorSchedules.UpdateAsync(schedule);
        await _unitOfWork.SaveChangesAsync();

        return Ok(schedule);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<IActionResult> Delete(int id)
    {
        var schedule = await _unitOfWork.DoctorSchedules.GetByIdAsync(id);
        if (schedule == null)
            return NotFound();

        schedule.IsAvailable = false;
        schedule.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.DoctorSchedules.UpdateAsync(schedule);
        await _unitOfWork.SaveChangesAsync();

        return Ok(new { message = "Schedule deleted" });
    }
}

public class CreateDoctorScheduleDto
{
    public int DoctorId { get; set; }
    public string DayOfWeek { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
}

