namespace HMS.Application.DTOs.Invoice;

public class InvoiceItemDto
{
    public int Id { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string? ItemType { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}

