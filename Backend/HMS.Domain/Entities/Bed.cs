using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Domain.Entities;

public class Bed
{
    public int Id { get; set; }
    
    public int RoomId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string BedNumber { get; set; } = string.Empty; // Unique identifier within room
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    [Required]
    public bool IsOccupied { get; set; } = false;
    
    public bool IsAvailable { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    [ForeignKey("RoomId")]
    public Room Room { get; set; } = null!;
    
    public ICollection<Admission> Admissions { get; set; } = new List<Admission>();
}

