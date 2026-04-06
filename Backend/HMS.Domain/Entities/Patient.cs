using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Domain.Entities;

public class Patient
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string PatientNumber { get; set; } = string.Empty; // Unique identifier
    
    [Required]
    public DateTime DateOfBirth { get; set; }
    
    [Required]
    [MaxLength(10)]
    public string Gender { get; set; } = string.Empty; // Male, Female, Other
    
    [MaxLength(500)]
    public string? Address { get; set; }
    
    [MaxLength(50)]
    public string? BloodGroup { get; set; }
    
    [MaxLength(1000)]
    public string? Allergies { get; set; }
    
    [MaxLength(1000)]
    public string? EmergencyContactName { get; set; }
    
    [MaxLength(20)]
    public string? EmergencyContactPhone { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
    
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
    public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    public ICollection<LabTest> LabTests { get; set; } = new List<LabTest>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    public ICollection<Admission> Admissions { get; set; } = new List<Admission>();
}

