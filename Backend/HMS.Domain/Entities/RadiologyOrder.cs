using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Domain.Entities;

public class RadiologyOrder
{
    public int Id { get; set; }
    
    [Required]
    public int PatientId { get; set; }
    
    [Required]
    public int DoctorId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Type { get; set; } = string.Empty; // X-Ray, MRI, CT, Ultrasound, Mammography
    
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Requested"; // Requested, Scheduled, InProgress, Completed, Cancelled
    
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? ScheduledAt { get; set; }
    
    public DateTime? CompletedAt { get; set; }
    
    public int? RadiologistId { get; set; } // Doctor who performed/interpreted
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal? Cost { get; set; }
    
    // Navigation properties
    [ForeignKey("PatientId")]
    public Patient Patient { get; set; } = null!;
    
    [ForeignKey("DoctorId")]
    public Doctor Doctor { get; set; } = null!;
    
    [ForeignKey("RadiologistId")]
    public Doctor? Radiologist { get; set; }
    
    public ICollection<RadiologyImage> Images { get; set; } = new List<RadiologyImage>();
    public RadiologyReport? Report { get; set; }
}

