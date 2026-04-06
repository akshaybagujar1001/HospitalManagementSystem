using System.ComponentModel.DataAnnotations;

namespace HMS.Domain.Entities;

public class InsuranceCompany
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? ContactInfo { get; set; }
    
    [MaxLength(100)]
    public string? PhoneNumber { get; set; }
    
    [MaxLength(100)]
    public string? Email { get; set; }
    
    [MaxLength(500)]
    public string? Address { get; set; }
    
    [MaxLength(2000)]
    public string? PolicyRules { get; set; } // JSON or text
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public ICollection<InsurancePolicy> InsurancePolicies { get; set; } = new List<InsurancePolicy>();
    public ICollection<InsuranceClaim> InsuranceClaims { get; set; } = new List<InsuranceClaim>();
}

