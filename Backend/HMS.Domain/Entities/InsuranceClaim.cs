using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Domain.Entities;

public class InsuranceClaim
{
    public int Id { get; set; }
    
    [Required]
    public int PatientId { get; set; }
    
    [Required]
    public int InvoiceId { get; set; }
    
    [Required]
    public int CompanyId { get; set; }
    
    [Required]
    public int PolicyId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected, Paid
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal AmountCovered { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal PatientResponsibility { get; set; }
    
    [MaxLength(1000)]
    public string? Notes { get; set; }
    
    public DateTime? SubmittedAt { get; set; }
    
    public DateTime? ProcessedAt { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    [ForeignKey("PatientId")]
    public Patient Patient { get; set; } = null!;
    
    [ForeignKey("InvoiceId")]
    public Invoice Invoice { get; set; } = null!;
    
    [ForeignKey("CompanyId")]
    public InsuranceCompany Company { get; set; } = null!;
    
    [ForeignKey("PolicyId")]
    public InsurancePolicy Policy { get; set; } = null!;
}

