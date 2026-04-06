namespace HMS.Application.DTOs.Doctor;

public class DoctorDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string DoctorNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public string? LicenseNumber { get; set; }
    public string? Bio { get; set; }
    public decimal ConsultationFee { get; set; }
    public bool IsAvailable { get; set; }
}

