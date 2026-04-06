namespace HMS.Application.DTOs.Finance;

public class ExpenseDto
{
    public int Id { get; set; }
    public string Category { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Vendor { get; set; }
    public string? PaymentMethod { get; set; }
    public DateTime ExpenseDate { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateExpenseDto
{
    public string Category { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Vendor { get; set; }
    public string? PaymentMethod { get; set; }
    public DateTime? ExpenseDate { get; set; }
    public string? Notes { get; set; }
}

public class UpdateExpenseDto
{
    public string Category { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Vendor { get; set; }
    public string? PaymentMethod { get; set; }
    public DateTime ExpenseDate { get; set; }
    public string? Notes { get; set; }
}

