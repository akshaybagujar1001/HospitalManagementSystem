namespace HMS.Application.DTOs.AIDiagnosis;

public class AIDiagnosisDto
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public string Symptoms { get; set; } = string.Empty;
    public string? VitalSigns { get; set; }
    public string? SuspectedDiagnosis { get; set; }
    public string? RecommendedTests { get; set; }
    public string RiskLevel { get; set; } = "Low";
    public DateTime CreatedAt { get; set; }
}

