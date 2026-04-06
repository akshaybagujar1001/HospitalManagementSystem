using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Domain.Entities;

public class PrescriptionFulfillment
{
    public int Id { get; set; }
    
    [Required]
    public int PrescriptionId { get; set; }
    
    [Required]
    public int MedicationId { get; set; }
    
    public int QuantityDispensed { get; set; }
    
    [Required]
    public int DispensedByUserId { get; set; }
    
    public DateTime DispensedAt { get; set; } = DateTime.UtcNow;
    
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    // Navigation properties
    [ForeignKey("PrescriptionId")]
    public Prescription Prescription { get; set; } = null!;
    
    [ForeignKey("MedicationId")]
    public Medication Medication { get; set; } = null!;
    
    [ForeignKey("DispensedByUserId")]
    public User DispensedBy { get; set; } = null!;
}

