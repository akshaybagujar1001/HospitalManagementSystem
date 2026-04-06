using System.ComponentModel.DataAnnotations;

namespace HMS.Application.DTOs.Patient;

public class CreatePatientDto
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;
    
    [Required]
    public DateTime DateOfBirth { get; set; }
    
    [Required]
    [MaxLength(10)]
    public string Gender { get; set; } = string.Empty;
    
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
}

