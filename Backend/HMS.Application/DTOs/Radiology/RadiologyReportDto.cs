namespace HMS.Application.DTOs.Radiology;

public class RadiologyReportDto
{
    public int Id { get; set; }
    public int RadiologyOrderId { get; set; }
    public int RadiologistId { get; set; }
    public string ReportText { get; set; } = string.Empty;
    public string? Findings { get; set; }
    public string? Impression { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? FinalizedAt { get; set; }
}

public class CreateRadiologyReportDto
{
    public int RadiologyOrderId { get; set; }
    public int RadiologistId { get; set; }
    public string ReportText { get; set; } = string.Empty;
    public string? Findings { get; set; }
    public string? Impression { get; set; }
    public string? Status { get; set; }
}

public class UpdateRadiologyReportDto
{
    public string ReportText { get; set; } = string.Empty;
    public string? Findings { get; set; }
    public string? Impression { get; set; }
    public string Status { get; set; } = string.Empty;
}

