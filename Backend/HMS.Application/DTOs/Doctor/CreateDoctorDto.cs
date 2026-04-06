using System.ComponentModel.DataAnnotations;

namespace HMS.Application.DTOs.Doctor;

public class CreateDoctorDto
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
    
    [MinLength(6)]
    public string? Password { get; set; }
    
    [MaxLength(20)]
    public string? PhoneNumber { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Specialization { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string? LicenseNumber { get; set; }
    
    [MaxLength(1000)]
    public string? Bio { get; set; }
    
    [Required]
    [Range(0, double.MaxValue)]
    public decimal ConsultationFee { get; set; }
    
    public bool IsAvailable { get; set; } = true;
}

