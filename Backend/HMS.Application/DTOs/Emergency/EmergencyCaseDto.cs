namespace HMS.Application.DTOs.Emergency;

public class EmergencyCaseDto
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int TriageLevel { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime ArrivalTime { get; set; }
    public int? AssignedDoctorId { get; set; }
    public int? AssignedNurseId { get; set; }
}

public class CreateEmergencyCaseDto
{
    public int PatientId { get; set; }
    public int TriageLevel { get; set; } = 3;
    public string Description { get; set; } = string.Empty;
    public string? ChiefComplaint { get; set; }
}

public class UpdateTriageDto
{
    public int TriageLevel { get; set; }
}

public class AssignStaffDto
{
    public int? DoctorId { get; set; }
    public int? NurseId { get; set; }
    public int? RoomId { get; set; }
}

