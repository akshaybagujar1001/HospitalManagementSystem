using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Domain.Entities;

public class Appointment
{
    public int Id { get; set; }
    
    public int PatientId { get; set; }
    
    public int DoctorId { get; set; }
    
    [Required]
    public DateTime AppointmentDate { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string AppointmentTime { get; set; } = string.Empty; // HH:mm format
    
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Scheduled"; // Scheduled, Completed, Cancelled, Rescheduled
    
    [MaxLength(500)]
    public string? Reason { get; set; }
    
    [MaxLength(1000)]
    public string? Notes { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    [ForeignKey("PatientId")]
    public Patient Patient { get; set; } = null!;
    
    [ForeignKey("DoctorId")]
    public Doctor Doctor { get; set; } = null!;
}

