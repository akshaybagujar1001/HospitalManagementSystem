using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Domain.Entities;

public class Asset
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string Type { get; set; } = string.Empty; // Medical Equipment, IT Equipment, Furniture, Vehicle, etc.
    
    [MaxLength(100)]
    public string? SerialNumber { get; set; }
    
    [MaxLength(200)]
    public string? Manufacturer { get; set; }
    
    [MaxLength(200)]
    public string? Model { get; set; }
    
    [MaxLength(500)]
    public string? Location { get; set; } // Room, Department, etc.
    
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Available"; // Available, InUse, Maintenance, Retired, Lost
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal? PurchasePrice { get; set; }
    
    public DateTime? PurchaseDate { get; set; }
    
    public DateTime? LastMaintenanceDate { get; set; }
    
    public DateTime? NextMaintenanceDate { get; set; }
    
    [MaxLength(1000)]
    public string? Notes { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public ICollection<AssetMaintenance> MaintenanceHistory { get; set; } = new List<AssetMaintenance>();
}

