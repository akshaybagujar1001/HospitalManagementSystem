using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Domain.Entities;

public class EmergencyCase
{
    public int Id { get; set; }
    
    [Required]
    public int PatientId { get; set; }
    
    [Required]
    [Range(1, 5)]
    public int TriageLevel { get; set; } = 3; // 1=Critical, 2=Urgent, 3=Moderate, 4=Less Urgent, 5=Non-Urgent
    
    [Required]
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string? ChiefComplaint { get; set; }
    
    public DateTime ArrivalTime { get; set; } = DateTime.UtcNow;
    
    public DateTime? TriageTime { get; set; }
    
    public DateTime? TreatmentStartTime { get; set; }
    
    public DateTime? DischargeTime { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Arrived"; // Arrived, Triage, Treatment, Discharged, Admitted, Deceased
    
    public int? AssignedDoctorId { get; set; }
    
    public int? AssignedNurseId { get; set; }
    
    public int? RoomId { get; set; }
    
    [MaxLength(2000)]
    public string? TreatmentNotes { get; set; }
    
    [MaxLength(1000)]
    public string? DischargeInstructions { get; set; }
    
    // Navigation properties
    [ForeignKey("PatientId")]
    public Patient Patient { get; set; } = null!;
    
    [ForeignKey("AssignedDoctorId")]
    public Doctor? AssignedDoctor { get; set; }
    
    [ForeignKey("AssignedNurseId")]
    public Nurse? AssignedNurse { get; set; }
    
    [ForeignKey("RoomId")]
    public Room? Room { get; set; }
}

