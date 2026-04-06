using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Domain.Entities;

public class Notification
{
    public int Id { get; set; }
    
    [Required]
    public int UserId { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(1000)]
    public string Message { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string Type { get; set; } = string.Empty; // Appointment, LabResult, Prescription, Billing, System, Emergency
    
    public bool IsRead { get; set; } = false;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? ReadAt { get; set; }
    
    // Optional: Link to related entity
    public string? RelatedEntityType { get; set; } // e.g., "Appointment", "Invoice"
    public int? RelatedEntityId { get; set; }
    
    // Navigation properties
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
}

