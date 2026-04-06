using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Domain.Entities;

public class AssetMaintenance
{
    public int Id { get; set; }
    
    [Required]
    public int AssetId { get; set; }
    
    [Required]
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string? MaintenanceType { get; set; } // Routine, Repair, Calibration, Inspection
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal? Cost { get; set; }
    
    public DateTime PerformedAt { get; set; } = DateTime.UtcNow;
    
    public int? PerformedByUserId { get; set; }
    
    [MaxLength(200)]
    public string? Vendor { get; set; }
    
    [MaxLength(2000)]
    public string? Notes { get; set; }
    
    // Navigation properties
    [ForeignKey("AssetId")]
    public Asset Asset { get; set; } = null!;
    
    [ForeignKey("PerformedByUserId")]
    public User? PerformedBy { get; set; }
}

