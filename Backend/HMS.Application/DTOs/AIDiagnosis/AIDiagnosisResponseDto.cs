namespace HMS.Application.DTOs.AIDiagnosis;

public class AIDiagnosisResponseDto
{
    public int Id { get; set; }
    public string SuspectedDiagnosis { get; set; } = string.Empty;
    public string RecommendedTests { get; set; } = string.Empty;
    public string RiskLevel { get; set; } = "Low";
    public double Confidence { get; set; }
    public DateTime CreatedAt { get; set; }
}

