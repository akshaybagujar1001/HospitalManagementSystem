using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Domain.Entities;

public class RevenueRecord
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string SourceType { get; set; } = string.Empty; // Invoice, Insurance, LabTest, Radiology, Other
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
    
    public int? InvoiceId { get; set; } // If source is Invoice
    
    public int? PatientId { get; set; }
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    [MaxLength(100)]
    public string? PaymentMethod { get; set; }
    
    public DateTime RevenueDate { get; set; } = DateTime.UtcNow;
    
    public int? CreatedByUserId { get; set; }
    
    [MaxLength(1000)]
    public string? Notes { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    [ForeignKey("InvoiceId")]
    public Invoice? Invoice { get; set; }
    
    [ForeignKey("PatientId")]
    public Patient? Patient { get; set; }
    
    [ForeignKey("CreatedByUserId")]
    public User? CreatedBy { get; set; }
}

