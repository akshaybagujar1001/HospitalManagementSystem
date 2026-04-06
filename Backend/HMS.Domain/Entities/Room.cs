using System.ComponentModel.DataAnnotations;

namespace HMS.Domain.Entities;

public class Room
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string RoomNumber { get; set; } = string.Empty; // Unique identifier
    
    [Required]
    [MaxLength(50)]
    public string RoomType { get; set; } = string.Empty; // General, ICU, Private, Semi-Private, etc.
    
    [MaxLength(100)]
    public string? Floor { get; set; }
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    [Required]
    public decimal PricePerDay { get; set; }
    
    public bool IsAvailable { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public ICollection<Bed> Beds { get; set; } = new List<Bed>();
}

