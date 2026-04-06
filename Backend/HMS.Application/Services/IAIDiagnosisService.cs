using HMS.Application.DTOs.AIDiagnosis;

namespace HMS.Application.Services;

public interface IAIDiagnosisService
{
    Task<AIDiagnosisResponseDto> AnalyzeSymptomsAsync(AIDiagnosisRequestDto request);
    Task<AIDiagnosisDto> GetDiagnosisAsync(int id);
    Task<IEnumerable<AIDiagnosisDto>> GetPatientDiagnosesAsync(int patientId);
}

