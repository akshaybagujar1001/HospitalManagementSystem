namespace HMS.Application.DTOs.NurseTask;

public class NurseTaskDto
{
    public int Id { get; set; }
    public int NurseId { get; set; }
    public int? PatientId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? DueTime { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateNurseTaskDto
{
    public int NurseId { get; set; }
    public int? PatientId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Priority { get; set; } = "Medium";
    public DateTime? DueTime { get; set; }
}

public class UpdateTaskStatusDto
{
    public string Status { get; set; } = string.Empty;
}

