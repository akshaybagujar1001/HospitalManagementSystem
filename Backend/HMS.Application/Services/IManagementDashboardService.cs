using HMS.Application.DTOs.Dashboard;

namespace HMS.Application.Services;

public interface IManagementDashboardService
{
    Task<DashboardOverviewDto> GetOverviewAsync();
    Task<FinancialDashboardDto> GetFinancialDataAsync(DateTime? startDate, DateTime? endDate);
    Task<OperationalDashboardDto> GetOperationalDataAsync(DateTime? startDate, DateTime? endDate);
}

