using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Domain.Entities;

public class InvoiceItem
{
    public int Id { get; set; }
    
    public int InvoiceId { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string ItemName { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string? ItemType { get; set; } // Consultation, LabTest, Medication, Room, etc.
    
    public int? ItemReferenceId { get; set; } // Reference to Appointment, LabTest, etc.
    
    [Required]
    public decimal Quantity { get; set; } = 1;
    
    [Required]
    public decimal UnitPrice { get; set; }
    
    [Required]
    public decimal TotalPrice { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    [ForeignKey("InvoiceId")]
    public Invoice Invoice { get; set; } = null!;
}

