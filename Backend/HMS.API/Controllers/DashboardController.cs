using HMS.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class DashboardController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(IUnitOfWork unitOfWork, ILogger<DashboardController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        try
        {
            var totalPatients = await _unitOfWork.Patients.CountAsync();
            var totalDoctors = await _unitOfWork.Doctors.CountAsync();
            var totalAppointments = await _unitOfWork.Appointments.CountAsync();
            var totalInvoices = await _unitOfWork.Invoices.CountAsync();

            return Ok(new
            {
                totalPatients,
                totalDoctors,
                totalAppointments,
                totalInvoices
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dashboard stats");
            return StatusCode(500, new { message = "An error occurred while getting dashboard statistics" });
        }
    }
}






