using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Domain.Entities;

public class Admission
{
    public int Id { get; set; }
    
    public int PatientId { get; set; }
    
    public int BedId { get; set; }
    
    public int? DoctorId { get; set; }
    
    public int? NurseId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string AdmissionNumber { get; set; } = string.Empty; // Unique identifier
    
    [Required]
    public DateTime AdmissionDate { get; set; } = DateTime.UtcNow;
    
    public DateTime? DischargeDate { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Admitted"; // Admitted, Discharged, Transferred
    
    [MaxLength(1000)]
    public string? Reason { get; set; }
    
    [MaxLength(2000)]
    public string? Notes { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    [ForeignKey("PatientId")]
    public Patient Patient { get; set; } = null!;
    
    [ForeignKey("BedId")]
    public Bed Bed { get; set; } = null!;
    
    [ForeignKey("DoctorId")]
    public Doctor? Doctor { get; set; }
    
    [ForeignKey("NurseId")]
    public Nurse? Nurse { get; set; }
}

