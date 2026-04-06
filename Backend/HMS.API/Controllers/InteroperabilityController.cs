using HMS.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.API.Controllers;

[ApiController]
[Route("api/interoperability")]
[Authorize(Roles = "Admin,Doctor")]
public class InteroperabilityController : ControllerBase
{
    private readonly IFhirExportService _fhirService;
    private readonly ILogger<InteroperabilityController> _logger;

    public InteroperabilityController(IFhirExportService fhirService, ILogger<InteroperabilityController> logger)
    {
        _fhirService = fhirService;
        _logger = logger;
    }

    [HttpGet("patient/{id}")]
    public async Task<IActionResult> GetFhirPatient(int id)
    {
        try
        {
            var patient = await _fhirService.GetFhirPatientAsync(id);
            if (patient == null)
                return NotFound();
            
            return Ok(patient);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting FHIR patient {PatientId}", id);
            return StatusCode(500, new { message = "An error occurred while exporting patient data" });
        }
    }

    [HttpGet("encounters/{patientId}")]
    public async Task<IActionResult> GetFhirEncounters(int patientId)
    {
        try
        {
            var encounters = await _fhirService.GetFhirEncountersAsync(patientId);
            return Ok(encounters);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting FHIR encounters for patient {PatientId}", patientId);
            return StatusCode(500, new { message = "An error occurred while exporting encounter data" });
        }
    }

    [HttpGet("observation/{labTestId}")]
    public async Task<IActionResult> GetFhirObservation(int labTestId)
    {
        try
        {
            var observation = await _fhirService.GetFhirObservationAsync(labTestId);
            if (observation == null)
                return NotFound();
            
            return Ok(observation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting FHIR observation {LabTestId}", labTestId);
            return StatusCode(500, new { message = "An error occurred while exporting observation data" });
        }
    }
}

