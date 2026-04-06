namespace HMS.Application.DTOs.Dashboard;

public class OperationalDashboardDto
{
    public double AverageWaitingTimeHours { get; set; }
    public double RoomOccupancyRate { get; set; }
    public int TotalLabTests { get; set; }
    public int TotalAppointments { get; set; }
    public int CompletedAppointments { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
}

