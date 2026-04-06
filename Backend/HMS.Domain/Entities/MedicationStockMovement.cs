using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Domain.Entities;

public class MedicationStockMovement
{
    public int Id { get; set; }
    
    [Required]
    public int MedicationId { get; set; }
    
    public int QuantityChange { get; set; } // Positive for In, Negative for Out
    
    [Required]
    [MaxLength(50)]
    public string MovementType { get; set; } = string.Empty; // In, Out, Adjustment, Expired
    
    [MaxLength(500)]
    public string? Reason { get; set; }
    
    public int? PerformedByUserId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    [ForeignKey("MedicationId")]
    public Medication Medication { get; set; } = null!;
    
    [ForeignKey("PerformedByUserId")]
    public User? PerformedBy { get; set; }
}

