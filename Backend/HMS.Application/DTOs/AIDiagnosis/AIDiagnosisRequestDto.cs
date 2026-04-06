using System.ComponentModel.DataAnnotations;

namespace HMS.Application.DTOs.AIDiagnosis;

public class AIDiagnosisRequestDto
{
    [Required]
    public int PatientId { get; set; }
    
    [Required]
    public int DoctorId { get; set; }
    
    [Required]
    public string Symptoms { get; set; } = string.Empty;
    
    public string? VitalSigns { get; set; } // JSON: {temperature, bloodPressure, heartRate, etc.}
}

