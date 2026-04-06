using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Domain.Entities;

public class Medication
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string? Code { get; set; } // Medication code (e.g., NDC)
    
    [MaxLength(100)]
    public string? DosageForm { get; set; } // Tablet, Capsule, Syrup, Injection, etc.
    
    [MaxLength(100)]
    public string? Strength { get; set; } // e.g., "500mg", "10ml"
    
    public int StockQuantity { get; set; } = 0;
    
    public int ReorderLevel { get; set; } = 10;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }
    
    public DateTime? ExpirationDate { get; set; }
    
    [MaxLength(500)]
    public string? Manufacturer { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public ICollection<MedicationStockMovement> StockMovements { get; set; } = new List<MedicationStockMovement>();
    public ICollection<PrescriptionFulfillment> PrescriptionFulfillments { get; set; } = new List<PrescriptionFulfillment>();
}

