using HMS.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.API.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize(Roles = "Admin")]
public class ManagementDashboardController : ControllerBase
{
    private readonly IManagementDashboardService _dashboardService;
    private readonly ILogger<ManagementDashboardController> _logger;

    public ManagementDashboardController(
        IManagementDashboardService dashboardService,
        ILogger<ManagementDashboardController> logger)
    {
        _dashboardService = dashboardService;
        _logger = logger;
    }

    [HttpGet("overview")]
    public async Task<IActionResult> GetOverview()
    {
        try
        {
            var overview = await _dashboardService.GetOverviewAsync();
            return Ok(overview);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dashboard overview");
            return StatusCode(500, new { message = "An error occurred while fetching dashboard data" });
        }
    }

    [HttpGet("financial")]
    public async Task<IActionResult> GetFinancialData([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        try
        {
            var financial = await _dashboardService.GetFinancialDataAsync(startDate, endDate);
            return Ok(financial);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting financial data");
            return StatusCode(500, new { message = "An error occurred while fetching financial data" });
        }
    }

    [HttpGet("operational")]
    public async Task<IActionResult> GetOperationalData([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        try
        {
            var operational = await _dashboardService.GetOperationalDataAsync(startDate, endDate);
            return Ok(operational);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting operational data");
            return StatusCode(500, new { message = "An error occurred while fetching operational data" });
        }
    }
}

