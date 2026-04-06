namespace HMS.Application.DTOs.Radiology;

public class RadiologyOrderDto
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime RequestedAt { get; set; }
    public DateTime? ScheduledAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int? RadiologistId { get; set; }
    public decimal? Cost { get; set; }
}

public class CreateRadiologyOrderDto
{
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal? Cost { get; set; }
}

public class UpdateRadiologyOrderStatusDto
{
    public string Status { get; set; } = string.Empty;
    public DateTime? ScheduledAt { get; set; }
    public int? RadiologistId { get; set; }
}

