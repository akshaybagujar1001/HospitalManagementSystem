using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Domain.Entities;

public class InsurancePolicy
{
    public int Id { get; set; }
    
    [Required]
    public int PatientId { get; set; }
    
    [Required]
    public int CompanyId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string PolicyNumber { get; set; } = string.Empty;
    
    [Range(0, 100)]
    public decimal CoveragePercentage { get; set; } // 0-100
    
    public DateTime? ExpirationDate { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    [MaxLength(1000)]
    public string? Notes { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    [ForeignKey("PatientId")]
    public Patient Patient { get; set; } = null!;
    
    [ForeignKey("CompanyId")]
    public InsuranceCompany Company { get; set; } = null!;
    
    public ICollection<InsuranceClaim> InsuranceClaims { get; set; } = new List<InsuranceClaim>();
}

