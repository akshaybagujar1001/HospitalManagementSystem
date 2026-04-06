using HMS.Application.DTOs.NurseTask;
using HMS.Domain.Entities;
using HMS.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HMS.API.Controllers;

[ApiController]
[Route("api/nurse-tasks")]
[Authorize]
public class NurseTasksController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<NurseTasksController> _logger;

    public NurseTasksController(IUnitOfWork unitOfWork, ILogger<NurseTasksController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetTasks([FromQuery] int? nurseId, [FromQuery] int? patientId, [FromQuery] string? status)
    {
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var tasks = await _unitOfWork.NurseTasks.FindAsync(t =>
            (nurseId == null || t.NurseId == nurseId) &&
            (patientId == null || t.PatientId == patientId) &&
            (status == null || t.Status == status) &&
            (userRole != "Nurse" || t.Nurse.UserId == userId) // Nurses only see their own tasks
        );

        var taskDtos = tasks.Select(t => new NurseTaskDto
        {
            Id = t.Id,
            NurseId = t.NurseId,
            PatientId = t.PatientId,
            Description = t.Description,
            Priority = t.Priority,
            Status = t.Status,
            DueTime = t.DueTime,
            CompletedAt = t.CompletedAt,
            CreatedAt = t.CreatedAt
        });

        return Ok(taskDtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTask(int id)
    {
        var task = await _unitOfWork.NurseTasks.GetByIdAsync(id);
        if (task == null)
            return NotFound();

        var taskDto = new NurseTaskDto
        {
            Id = task.Id,
            NurseId = task.NurseId,
            PatientId = task.PatientId,
            Description = task.Description,
            Priority = task.Priority,
            Status = task.Status,
            DueTime = task.DueTime,
            CompletedAt = task.CompletedAt,
            CreatedAt = task.CreatedAt
        };

        return Ok(taskDto);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<IActionResult> CreateTask([FromBody] CreateNurseTaskDto createDto)
    {
        try
        {
            var task = new NurseTask
            {
                NurseId = createDto.NurseId,
                PatientId = createDto.PatientId,
                Description = createDto.Description,
                Priority = createDto.Priority,
                Status = "Pending",
                DueTime = createDto.DueTime,
                AssignedByUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0"),
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.NurseTasks.AddAsync(task);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating nurse task");
            return StatusCode(500, new { message = "An error occurred while creating the task" });
        }
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Nurse,Admin")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateTaskStatusDto updateDto)
    {
        var task = await _unitOfWork.NurseTasks.GetByIdAsync(id);
        if (task == null)
            return NotFound();

        task.Status = updateDto.Status;
        if (updateDto.Status == "Completed")
        {
            task.CompletedAt = DateTime.UtcNow;
        }
        task.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.NurseTasks.UpdateAsync(task);
        await _unitOfWork.SaveChangesAsync();

        return Ok(new { message = "Task status updated" });
    }
}

