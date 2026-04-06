namespace HMS.Application.DTOs.Insurance;

public class InsuranceClaimDto
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int InvoiceId { get; set; }
    public int CompanyId { get; set; }
    public int PolicyId { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal AmountCovered { get; set; }
    public decimal PatientResponsibility { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateInsuranceClaimDto
{
    public int InvoiceId { get; set; }
    public int PolicyId { get; set; }
}

public class UpdateClaimStatusDto
{
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

