using System.ComponentModel.DataAnnotations;

namespace HMS.Domain.Entities;

public class LabTestType
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    [Required]
    public decimal Price { get; set; }
    
    [MaxLength(50)]
    public string? Category { get; set; } // Blood Test, Urine Test, X-Ray, etc.
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public ICollection<LabTest> LabTests { get; set; } = new List<LabTest>();
}

