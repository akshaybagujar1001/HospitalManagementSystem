using HMS.Application.DTOs.AIDiagnosis;
using HMS.Domain.Entities;
using HMS.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace HMS.Application.Services;

public class AIDiagnosisService : IAIDiagnosisService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AIDiagnosisService> _logger;

    public AIDiagnosisService(IUnitOfWork unitOfWork, ILogger<AIDiagnosisService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<AIDiagnosisResponseDto> AnalyzeSymptomsAsync(AIDiagnosisRequestDto request)
    {
        // Mock AI Analysis - In production, this would call OpenAI, Azure AI, or similar
        var analysis = PerformMockAnalysis(request);

        // Save to database
        var aiDiagnosis = new AIDiagnosis
        {
            PatientId = request.PatientId,
            DoctorId = request.DoctorId,
            Symptoms = request.Symptoms,
            VitalSigns = request.VitalSigns,
            SuspectedDiagnosis = analysis.SuspectedDiagnosis,
            RecommendedTests = analysis.RecommendedTests,
            RiskLevel = analysis.RiskLevel,
            AIResponse = System.Text.Json.JsonSerializer.Serialize(analysis),
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.AIDiagnoses.AddAsync(aiDiagnosis);
        await _unitOfWork.SaveChangesAsync();

        return new AIDiagnosisResponseDto
        {
            Id = aiDiagnosis.Id,
            SuspectedDiagnosis = analysis.SuspectedDiagnosis,
            RecommendedTests = analysis.RecommendedTests,
            RiskLevel = analysis.RiskLevel,
            Confidence = analysis.Confidence,
            CreatedAt = aiDiagnosis.CreatedAt
        };
    }

    private MockAnalysisResult PerformMockAnalysis(AIDiagnosisRequestDto request)
    {
        // Mock AI logic - analyze symptoms and return suggestions
        var symptoms = request.Symptoms.ToLower();
        var riskLevel = "Low";
        var suspectedDiagnosis = new List<string>();
        var recommendedTests = new List<string>();

        // Simple pattern matching (in production, use real AI)
        if (symptoms.Contains("fever") && symptoms.Contains("cough"))
        {
            suspectedDiagnosis.Add("Upper Respiratory Infection");
            suspectedDiagnosis.Add("Influenza");
            recommendedTests.Add("Complete Blood Count (CBC)");
            recommendedTests.Add("Chest X-Ray");
            riskLevel = "Medium";
        }
        else if (symptoms.Contains("chest pain"))
        {
            suspectedDiagnosis.Add("Cardiac Evaluation Needed");
            recommendedTests.Add("ECG");
            recommendedTests.Add("Cardiac Enzymes");
            riskLevel = "High";
        }
        else if (symptoms.Contains("headache") && symptoms.Contains("nausea"))
        {
            suspectedDiagnosis.Add("Migraine");
            suspectedDiagnosis.Add("Tension Headache");
            recommendedTests.Add("Blood Pressure");
            riskLevel = "Low";
        }
        else
        {
            suspectedDiagnosis.Add("General Examination Required");
            recommendedTests.Add("Vital Signs");
            riskLevel = "Low";
        }

        return new MockAnalysisResult
        {
            SuspectedDiagnosis = string.Join(", ", suspectedDiagnosis),
            RecommendedTests = string.Join(", ", recommendedTests),
            RiskLevel = riskLevel,
            Confidence = 0.75 // Mock confidence score
        };
    }

    public async Task<AIDiagnosisDto> GetDiagnosisAsync(int id)
    {
        var diagnosis = await _unitOfWork.AIDiagnoses.GetByIdAsync(id);
        if (diagnosis == null) return null!;

        return new AIDiagnosisDto
        {
            Id = diagnosis.Id,
            PatientId = diagnosis.PatientId,
            DoctorId = diagnosis.DoctorId,
            Symptoms = diagnosis.Symptoms,
            VitalSigns = diagnosis.VitalSigns,
            SuspectedDiagnosis = diagnosis.SuspectedDiagnosis,
            RecommendedTests = diagnosis.RecommendedTests,
            RiskLevel = diagnosis.RiskLevel,
            CreatedAt = diagnosis.CreatedAt
        };
    }

    public async Task<IEnumerable<AIDiagnosisDto>> GetPatientDiagnosesAsync(int patientId)
    {
        var diagnoses = await _unitOfWork.AIDiagnoses.FindAsync(d => d.PatientId == patientId);
        return diagnoses.Select(d => new AIDiagnosisDto
        {
            Id = d.Id,
            PatientId = d.PatientId,
            DoctorId = d.DoctorId,
            Symptoms = d.Symptoms,
            VitalSigns = d.VitalSigns,
            SuspectedDiagnosis = d.SuspectedDiagnosis,
            RecommendedTests = d.RecommendedTests,
            RiskLevel = d.RiskLevel,
            CreatedAt = d.CreatedAt
        });
    }

    private class MockAnalysisResult
    {
        public string SuspectedDiagnosis { get; set; } = string.Empty;
        public string RecommendedTests { get; set; } = string.Empty;
        public string RiskLevel { get; set; } = "Low";
        public double Confidence { get; set; }
    }
}

