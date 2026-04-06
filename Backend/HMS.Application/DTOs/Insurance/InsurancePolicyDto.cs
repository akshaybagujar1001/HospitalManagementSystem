namespace HMS.Application.DTOs.Insurance;

public class InsurancePolicyDto
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int CompanyId { get; set; }
    public string PolicyNumber { get; set; } = string.Empty;
    public decimal CoveragePercentage { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateInsurancePolicyDto
{
    public int PatientId { get; set; }
    public int CompanyId { get; set; }
    public string PolicyNumber { get; set; } = string.Empty;
    public decimal CoveragePercentage { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string? Notes { get; set; }
}

public class UpdateInsurancePolicyDto
{
    public decimal CoveragePercentage { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public bool IsActive { get; set; }
    public string? Notes { get; set; }
}

