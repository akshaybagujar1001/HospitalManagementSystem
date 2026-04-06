namespace HMS.Application.DTOs.Shift;

public class StaffShiftDto
{
    public int Id { get; set; }
    public int StaffId { get; set; }
    public string Role { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? Department { get; set; }
    public bool IsNightShift { get; set; }
}

public class CreateStaffShiftDto
{
    public int StaffId { get; set; }
    public string Role { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? Department { get; set; }
    public string? Notes { get; set; }
}

public class UpdateStaffShiftDto
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? Department { get; set; }
    public string? Notes { get; set; }
}

