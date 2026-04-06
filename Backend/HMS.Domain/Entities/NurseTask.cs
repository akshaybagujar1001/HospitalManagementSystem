using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Domain.Entities;

public class NurseTask
{
    public int Id { get; set; }
    
    [Required]
    public int NurseId { get; set; }
    
    public int? PatientId { get; set; } // Nullable for non-patient tasks
    
    [Required]
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string Priority { get; set; } = "Medium"; // Low, Medium, High, Critical
    
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Pending"; // Pending, InProgress, Completed, Cancelled
    
    public DateTime? DueTime { get; set; }
    
    public DateTime? CompletedAt { get; set; }
    
    public int? AssignedByUserId { get; set; } // Doctor or Admin who assigned
    
    [MaxLength(1000)]
    public string? Notes { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    [ForeignKey("NurseId")]
    public Nurse Nurse { get; set; } = null!;
    
    [ForeignKey("PatientId")]
    public Patient? Patient { get; set; }
    
    [ForeignKey("AssignedByUserId")]
    public User? AssignedBy { get; set; }
}

