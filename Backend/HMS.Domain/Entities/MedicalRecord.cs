using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Domain.Entities;

public class MedicalRecord
{
    public int Id { get; set; }
    
    public int PatientId { get; set; }
    
    public int DoctorId { get; set; }
    
    public int? AppointmentId { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Diagnosis { get; set; } = string.Empty;
    
    [MaxLength(2000)]
    public string? Symptoms { get; set; }
    
    [MaxLength(2000)]
    public string? Treatment { get; set; }
    
    [MaxLength(2000)]
    public string? Notes { get; set; }
    
    [Required]
    public DateTime RecordDate { get; set; } = DateTime.UtcNow;
    
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

