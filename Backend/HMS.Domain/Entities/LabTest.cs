using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Domain.Entities;

public class LabTest
{
    public int Id { get; set; }
    
    public int PatientId { get; set; }
    
    public int DoctorId { get; set; }
    
    public int LabTestTypeId { get; set; }
    
    public int? AppointmentId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string TestNumber { get; set; } = string.Empty; // Unique identifier
    
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Pending"; // Pending, InProgress, Completed, Cancelled
    
    [MaxLength(2000)]
    public string? Results { get; set; }
    
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    public DateTime RequestedDate { get; set; } = DateTime.UtcNow;
    
    public DateTime? CompletedDate { get; set; }
    
    public decimal Price { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    [ForeignKey("PatientId")]
    public Patient Patient { get; set; } = null!;
    
    [ForeignKey("DoctorId")]
    public Doctor Doctor { get; set; } = null!;
    
    [ForeignKey("LabTestTypeId")]
    public LabTestType LabTestType { get; set; } = null!;
    
    [ForeignKey("AppointmentId")]
    public Appointment? Appointment { get; set; }
}

