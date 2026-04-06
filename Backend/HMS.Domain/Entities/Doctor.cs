using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Domain.Entities;

public class Doctor
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string DoctorNumber { get; set; } = string.Empty; // Unique identifier
    
    [Required]
    [MaxLength(100)]
    public string Specialization { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string? LicenseNumber { get; set; }
    
    [MaxLength(20)]
    public string? PhoneNumber { get; set; }
    
    [MaxLength(1000)]
    public string? Bio { get; set; }
    
    public decimal ConsultationFee { get; set; }
    
    public bool IsAvailable { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
    
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
    public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    public ICollection<LabTest> LabTests { get; set; } = new List<LabTest>();
    public ICollection<DoctorSchedule> Schedules { get; set; } = new List<DoctorSchedule>();
}

