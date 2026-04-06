using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Domain.Entities;

public class Expense
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Category { get; set; } = string.Empty; // Salaries, Equipment, Supplies, Utilities, Maintenance, etc.
    
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
    
    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string? Vendor { get; set; }
    
    [MaxLength(100)]
    public string? PaymentMethod { get; set; } // Cash, Bank Transfer, Credit Card
    
    public DateTime ExpenseDate { get; set; } = DateTime.UtcNow;
    
    public int? CreatedByUserId { get; set; }
    
    [MaxLength(1000)]
    public string? Notes { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    [ForeignKey("CreatedByUserId")]
    public User? CreatedBy { get; set; }
}

