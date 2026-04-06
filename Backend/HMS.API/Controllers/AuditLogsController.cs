using HMS.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HMS.API.Controllers;

[ApiController]
[Route("api/audit-logs")]
[Authorize(Roles = "Admin")]
public class AuditLogsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly HMS.Infrastructure.Data.HmsDbContext _context;
    private readonly ILogger<AuditLogsController> _logger;

    public AuditLogsController(
        IUnitOfWork unitOfWork,
        HMS.Infrastructure.Data.HmsDbContext context,
        ILogger<AuditLogsController> logger)
    {
        _unitOfWork = unitOfWork;
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAuditLogs(
        [FromQuery] string? entityName,
        [FromQuery] int? entityId,
        [FromQuery] int? userId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var query = _context.AuditLogs.AsQueryable();

        if (!string.IsNullOrEmpty(entityName))
            query = query.Where(a => a.EntityName == entityName);
        if (entityId.HasValue)
            query = query.Where(a => a.EntityId == entityId);
        if (userId.HasValue)
            query = query.Where(a => a.UserId == userId);
        if (startDate.HasValue)
            query = query.Where(a => a.Timestamp >= startDate.Value);
        if (endDate.HasValue)
            query = query.Where(a => a.Timestamp <= endDate.Value);

        var total = await query.CountAsync();
        var logs = await query
            .OrderByDescending(a => a.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new
        {
            data = logs,
            total,
            page,
            pageSize,
            totalPages = (int)Math.Ceiling(total / (double)pageSize)
        });
    }

    [HttpGet("entity/{entityName}/{entityId}")]
    public async Task<IActionResult> GetEntityHistory(string entityName, int entityId)
    {
        var logs = await _unitOfWork.AuditLogs.FindAsync(a =>
            a.EntityName == entityName && a.EntityId == entityId);

        return Ok(logs.OrderBy(a => a.Timestamp));
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserActivity(int userId)
    {
        var logs = await _unitOfWork.AuditLogs.FindAsync(a => a.UserId == userId);
        return Ok(logs.OrderByDescending(a => a.Timestamp));
    }
}

