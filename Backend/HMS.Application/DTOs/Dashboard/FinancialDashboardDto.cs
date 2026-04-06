namespace HMS.Application.DTOs.Dashboard;

public class FinancialDashboardDto
{
    public decimal TotalRevenue { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal NetProfit { get; set; }
    public int TotalInvoices { get; set; }
    public int PaidInvoices { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
}

