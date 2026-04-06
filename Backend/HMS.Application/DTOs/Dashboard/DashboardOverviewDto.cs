namespace HMS.Application.DTOs.Dashboard;

public class DashboardOverviewDto
{
    public int TotalPatients { get; set; }
    public int TotalDoctors { get; set; }
    public int TotalNurses { get; set; }
    public int MonthlyNewPatients { get; set; }
    public int ActiveInpatients { get; set; }
    public int TodayAppointments { get; set; }
    public int AvailableRooms { get; set; }
}

