using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Domain.Entities;

public class AIDiagnosis
{
    public int Id { get; set; }
    
    [Required]
    public int PatientId { get; set; }
    
    [Required]
    public int DoctorId { get; set; }
    
    [Required]
    public string Symptoms { get; set; } = string.Empty; // JSON or text
    
    [MaxLength(500)]
    public string? VitalSigns { get; set; } // JSON: {temperature, bloodPressure, heartRate, etc.}
    
    [MaxLength(1000)]
    public string? SuspectedDiagnosis { get; set; } // Comma-separated or JSON
    
    [MaxLength(1000)]
    public string? RecommendedTests { get; set; } // Comma-separated or JSON
    
    [Required]
    [MaxLength(50)]
    public string RiskLevel { get; set; } = "Low"; // Low, Medium, High
    
    [MaxLength(2000)]
    public string? AIResponse { get; set; } // Full AI response (JSON)
    
    [MaxLength(1000)]
    public string? Notes { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    [ForeignKey("PatientId")]
    public Patient Patient { get; set; } = null!;
    
    [ForeignKey("DoctorId")]
    public Doctor Doctor { get; set; } = null!;
}

