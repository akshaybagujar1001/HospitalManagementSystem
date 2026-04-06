using System.ComponentModel.DataAnnotations;

namespace HMS.Application.DTOs.Appointment;

public class CreateAppointmentDto
{
    [Required]
    public int PatientId { get; set; }
    
    [Required]
    public int DoctorId { get; set; }
    
    [Required]
    public DateTime AppointmentDate { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string AppointmentTime { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Reason { get; set; }
}

