using HMS.Application.DTOs.AIDiagnosis;
using HMS.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.API.Controllers;

[ApiController]
[Route("api/ai-diagnosis")]
[Authorize(Roles = "Doctor,Admin")]
public class AIDiagnosisController : ControllerBase
{
    private readonly IAIDiagnosisService _aiDiagnosisService;
    private readonly ILogger<AIDiagnosisController> _logger;

    public AIDiagnosisController(IAIDiagnosisService aiDiagnosisService, ILogger<AIDiagnosisController> logger)
    {
        _aiDiagnosisService = aiDiagnosisService;
        _logger = logger;
    }

    [HttpPost("analyze")]
    public async Task<IActionResult> AnalyzeSymptoms([FromBody] AIDiagnosisRequestDto request)
    {
        try
        {
            var result = await _aiDiagnosisService.AnalyzeSymptomsAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing symptoms");
            return StatusCode(500, new { message = "An error occurred while analyzing symptoms" });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDiagnosis(int id)
    {
        var diagnosis = await _aiDiagnosisService.GetDiagnosisAsync(id);
        if (diagnosis == null)
            return NotFound();
        
        return Ok(diagnosis);
    }

    [HttpGet("patient/{patientId}")]
    public async Task<IActionResult> GetPatientDiagnoses(int patientId)
    {
        var diagnoses = await _aiDiagnosisService.GetPatientDiagnosesAsync(patientId);
        return Ok(diagnoses);
    }
}

