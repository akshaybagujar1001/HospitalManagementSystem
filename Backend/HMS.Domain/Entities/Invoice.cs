using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Domain.Entities;

public class Invoice
{
    public int Id { get; set; }
    
    public int PatientId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string InvoiceNumber { get; set; } = string.Empty; // Unique identifier
    
    [Required]
    public decimal TotalAmount { get; set; }
    
    public decimal? DiscountAmount { get; set; }
    
    [Required]
    public decimal FinalAmount { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Pending"; // Pending, Paid, PartiallyPaid, Cancelled
    
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    [Required]
    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;
    
    public DateTime? DueDate { get; set; }
    
    public DateTime? PaidDate { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    [ForeignKey("PatientId")]
    public Patient Patient { get; set; } = null!;
    
    public ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
}

