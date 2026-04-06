using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Domain.Entities;

public class AuditLog
{
    public int Id { get; set; }
    
    public int? UserId { get; set; } // Nullable for system actions
    
    [Required]
    [MaxLength(50)]
    public string Action { get; set; } = string.Empty; // Create, Update, Delete
    
    [Required]
    [MaxLength(100)]
    public string EntityName { get; set; } = string.Empty; // Patient, Appointment, etc.
    
    public int? EntityId { get; set; }
    
    public string? OldValues { get; set; } // JSON string
    
    public string? NewValues { get; set; } // JSON string
    
    [MaxLength(50)]
    public string? IPAddress { get; set; }
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    [ForeignKey("UserId")]
    public User? User { get; set; }
}

