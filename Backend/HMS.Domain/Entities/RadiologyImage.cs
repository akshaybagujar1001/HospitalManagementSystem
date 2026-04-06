using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Domain.Entities;

public class RadiologyImage
{
    public int Id { get; set; }
    
    [Required]
    public int RadiologyOrderId { get; set; }
    
    [Required]
    [MaxLength(500)]
    public string FilePath { get; set; } = string.Empty; // Path or URL to image
    
    [MaxLength(200)]
    public string? FileName { get; set; }
    
    [MaxLength(50)]
    public string? FileType { get; set; } // DICOM, JPEG, PNG
    
    public long? FileSize { get; set; } // in bytes
    
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    
    public int? UploadedByUserId { get; set; }
    
    // Navigation properties
    [ForeignKey("RadiologyOrderId")]
    public RadiologyOrder RadiologyOrder { get; set; } = null!;
    
    [ForeignKey("UploadedByUserId")]
    public User? UploadedBy { get; set; }
}

