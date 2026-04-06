using HMS.Application.DTOs.Fhir;

namespace HMS.Application.Services;

public interface IFhirExportService
{
    Task<FhirPatientDto> GetFhirPatientAsync(int patientId);
    Task<IEnumerable<FhirEncounterDto>> GetFhirEncountersAsync(int patientId);
    Task<FhirObservationDto> GetFhirObservationAsync(int labTestId);
}

