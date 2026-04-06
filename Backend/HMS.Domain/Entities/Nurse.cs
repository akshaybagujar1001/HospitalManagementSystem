using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Domain.Entities;

public class Nurse
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string NurseNumber { get; set; } = string.Empty; // Unique identifier
    
    [MaxLength(50)]
    public string? LicenseNumber { get; set; }
    
    [MaxLength(20)]
    public string? PhoneNumber { get; set; }
    
    [MaxLength(100)]
    public string? Department { get; set; }
    
    public bool IsAvailable { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
    
    public ICollection<Admission> Admissions { get; set; } = new List<Admission>();
}

