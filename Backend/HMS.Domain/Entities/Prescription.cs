using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Domain.Entities;

public class Prescription
{
    public int Id { get; set; }
    
    public int PatientId { get; set; }
    
    public int DoctorId { get; set; }
    
    public int? AppointmentId { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string MedicationName { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Dosage { get; set; }
    
    [MaxLength(500)]
    public string? Instructions { get; set; }
    
    [MaxLength(50)]
    public string? Frequency { get; set; } // e.g., "Twice daily", "Once a week"
    
    public int? Quantity { get; set; }
    
    public int? DurationDays { get; set; }
    
    [Required]
    public DateTime PrescribedDate { get; set; } = DateTime.UtcNow;
    
    public DateTime? ExpiryDate { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    [ForeignKey("PatientId")]
    public Patient Patient { get; set; } = null!;
    
    [ForeignKey("DoctorId")]
    public Doctor Doctor { get; set; } = null!;
    
    [ForeignKey("AppointmentId")]
    public Appointment? Appointment { get; set; }
}

