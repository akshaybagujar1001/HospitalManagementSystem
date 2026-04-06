using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Domain.Entities;

public class RadiologyReport
{
    public int Id { get; set; }
    
    [Required]
    public int RadiologyOrderId { get; set; }
    
    [Required]
    public int RadiologistId { get; set; }
    
    [Required]
    public string ReportText { get; set; } = string.Empty; // Full report content
    
    [MaxLength(200)]
    public string? Findings { get; set; }
    
    [MaxLength(200)]
    public string? Impression { get; set; }
    
    [MaxLength(50)]
    public string? Status { get; set; } = "Draft"; // Draft, Final, Amended
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? FinalizedAt { get; set; }
    
    // Navigation properties
    [ForeignKey("RadiologyOrderId")]
    public RadiologyOrder RadiologyOrder { get; set; } = null!;
    
    [ForeignKey("RadiologistId")]
    public Doctor Radiologist { get; set; } = null!;
}

