namespace HMS.Application.DTOs.Radiology;

public class RadiologyImageDto
{
    public int Id { get; set; }
    public int RadiologyOrderId { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string? FileName { get; set; }
    public string? FileType { get; set; }
    public long? FileSize { get; set; }
    public string? Description { get; set; }
    public DateTime UploadedAt { get; set; }
}

public class CreateRadiologyImageDto
{
    public int RadiologyOrderId { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string? FileName { get; set; }
    public string? FileType { get; set; }
    public long? FileSize { get; set; }
    public string? Description { get; set; }
}

