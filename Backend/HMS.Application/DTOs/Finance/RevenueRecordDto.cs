namespace HMS.Application.DTOs.Finance;

public class RevenueRecordDto
{
    public int Id { get; set; }
    public string SourceType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int? InvoiceId { get; set; }
    public int? PatientId { get; set; }
    public string? Description { get; set; }
    public string? PaymentMethod { get; set; }
    public DateTime RevenueDate { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateRevenueRecordDto
{
    public string SourceType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int? InvoiceId { get; set; }
    public int? PatientId { get; set; }
    public string? Description { get; set; }
    public string? PaymentMethod { get; set; }
    public DateTime? RevenueDate { get; set; }
    public string? Notes { get; set; }
}

