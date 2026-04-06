using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Domain.Entities;

public class DoctorSchedule
{
    public int Id { get; set; }
    
    public int DoctorId { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string DayOfWeek { get; set; } = string.Empty; // Monday, Tuesday, etc.
    
    [Required]
    [MaxLength(20)]
    public string StartTime { get; set; } = string.Empty; // HH:mm format
    
    [Required]
    [MaxLength(20)]
    public string EndTime { get; set; } = string.Empty; // HH:mm format
    
    public bool IsAvailable { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    [ForeignKey("DoctorId")]
    public Doctor Doctor { get; set; } = null!;
}

