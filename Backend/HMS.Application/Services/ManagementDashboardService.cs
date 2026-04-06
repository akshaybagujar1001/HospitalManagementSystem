using HMS.Application.DTOs.Dashboard;
using HMS.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HMS.Application.Services;

public class ManagementDashboardService : IManagementDashboardService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly DbContext _context;

    public ManagementDashboardService(IUnitOfWork unitOfWork, DbContext context)
    {
        _unitOfWork = unitOfWork;
        _context = context;
    }

    public async Task<DashboardOverviewDto> GetOverviewAsync()
    {
        var totalPatients = await _unitOfWork.Patients.CountAsync();
        var totalDoctors = await _unitOfWork.Doctors.CountAsync();
        var totalNurses = await _unitOfWork.Nurses.CountAsync();
        
        var monthlyNewPatients = await _context.Set<HMS.Domain.Entities.Patient>()
            .CountAsync(p => p.CreatedAt >= DateTime.UtcNow.AddMonths(-1));
        
        var activeInpatients = await _context.Set<HMS.Domain.Entities.Admission>()
            .CountAsync(a => a.Status == "Active");
        
        var todayAppointments = await _context.Set<HMS.Domain.Entities.Appointment>()
            .CountAsync(a => a.AppointmentDate.Date == DateTime.UtcNow.Date);
        
        var availableRooms = await _context.Set<HMS.Domain.Entities.Room>()
            .CountAsync(r => r.IsAvailable);
        
        return new DashboardOverviewDto
        {
            TotalPatients = totalPatients,
            TotalDoctors = totalDoctors,
            TotalNurses = totalNurses,
            MonthlyNewPatients = monthlyNewPatients,
            ActiveInpatients = activeInpatients,
            TodayAppointments = todayAppointments,
            AvailableRooms = availableRooms
        };
    }

    public async Task<FinancialDashboardDto> GetFinancialDataAsync(DateTime? startDate, DateTime? endDate)
    {
        var start = startDate ?? DateTime.UtcNow.AddMonths(-1);
        var end = endDate ?? DateTime.UtcNow;
        
        decimal revenue = 0;
        decimal expenses = 0;
        int invoices = 0;
        int paidInvoices = 0;
        
        try
        {
            revenue = await _context.Set<HMS.Domain.Entities.RevenueRecord>()
                .Where(r => r.RevenueDate >= start && r.RevenueDate <= end)
                .SumAsync(r => (decimal?)r.Amount) ?? 0;
        }
        catch (Exception ex) when (IsTableNotFoundError(ex))
        {
            // Table doesn't exist yet, revenue will remain 0
        }
        
        try
        {
            expenses = await _context.Set<HMS.Domain.Entities.Expense>()
                .Where(e => e.ExpenseDate >= start && e.ExpenseDate <= end)
                .SumAsync(e => (decimal?)e.Amount) ?? 0;
        }
        catch (Exception ex) when (IsTableNotFoundError(ex))
        {
            // Table doesn't exist yet, expenses will remain 0
        }
        
        try
        {
            invoices = await _context.Set<HMS.Domain.Entities.Invoice>()
                .Where(i => i.CreatedAt >= start && i.CreatedAt <= end)
                .CountAsync();
            
            paidInvoices = await _context.Set<HMS.Domain.Entities.Invoice>()
                .Where(i => i.CreatedAt >= start && i.CreatedAt <= end && i.Status == "Paid")
                .CountAsync();
        }
        catch (Exception ex) when (IsTableNotFoundError(ex))
        {
            // Table doesn't exist yet, counts will remain 0
        }
        
        return new FinancialDashboardDto
        {
            TotalRevenue = revenue,
            TotalExpenses = expenses,
            NetProfit = revenue - expenses,
            TotalInvoices = invoices,
            PaidInvoices = paidInvoices,
            PeriodStart = start,
            PeriodEnd = end
        };
    }

    public async Task<OperationalDashboardDto> GetOperationalDataAsync(DateTime? startDate, DateTime? endDate)
    {
        var start = startDate ?? DateTime.UtcNow.AddMonths(-1);
        var end = endDate ?? DateTime.UtcNow;
        
        var appointments = await _context.Set<HMS.Domain.Entities.Appointment>()
            .Where(a => a.AppointmentDate >= start && a.AppointmentDate <= end)
            .ToListAsync();
        
        var avgWaitingTime = appointments.Any() 
            ? appointments.Where(a => a.Status == "Completed")
                .Average(a => (a.UpdatedAt - a.CreatedAt)?.TotalHours ?? 0)
            : 0;
        
        var roomOccupancy = await _context.Set<HMS.Domain.Entities.Room>()
            .ToListAsync();
        
        var totalRooms = roomOccupancy.Count;
        var occupiedRooms = roomOccupancy.Count(r => !r.IsAvailable);
        var occupancyRate = totalRooms > 0 ? (double)occupiedRooms / totalRooms * 100 : 0;
        
        var labTests = await _context.Set<HMS.Domain.Entities.LabTest>()
            .CountAsync(lt => lt.CreatedAt >= start && lt.CreatedAt <= end);
        
        return new OperationalDashboardDto
        {
            AverageWaitingTimeHours = avgWaitingTime,
            RoomOccupancyRate = occupancyRate,
            TotalLabTests = labTests,
            TotalAppointments = appointments.Count,
            CompletedAppointments = appointments.Count(a => a.Status == "Completed"),
            PeriodStart = start,
            PeriodEnd = end
        };
    }
    
    private static bool IsTableNotFoundError(Exception ex)
    {
        // Check for SQL Server error 208 (Invalid object name)
        // This works by checking the inner exception message
        var innerEx = ex;
        while (innerEx != null)
        {
            var message = innerEx.Message;
            if (message.Contains("Invalid object name", StringComparison.OrdinalIgnoreCase) ||
                message.Contains("208", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            innerEx = innerEx.InnerException;
        }
        return false;
    }
}

