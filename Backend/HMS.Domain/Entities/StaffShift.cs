using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Domain.Entities;

public class StaffShift
{
    public int Id { get; set; }
    
    [Required]
    public int StaffId { get; set; } // User ID (Doctor or Nurse)
    
    [Required]
    [MaxLength(50)]
    public string Role { get; set; } = string.Empty; // Doctor, Nurse
    
    [Required]
    public DateTime StartTime { get; set; }
    
    [Required]
    public DateTime EndTime { get; set; }
    
    [MaxLength(100)]
    public string? Department { get; set; }
    
    public bool IsNightShift { get; set; } = false;
    
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public int? CreatedByUserId { get; set; } // Admin who created the shift
    
    // Navigation properties
    [ForeignKey("StaffId")]
    public User Staff { get; set; } = null!;
    
    [ForeignKey("CreatedByUserId")]
    public User? CreatedBy { get; set; }
}

