using HMS.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HMS.API.Controllers;

[ApiController]
[Route("api/financial-reports")]
[Authorize(Roles = "Admin,Finance")]
public class FinancialReportsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly HMS.Infrastructure.Data.HmsDbContext _context;
    private readonly ILogger<FinancialReportsController> _logger;

    public FinancialReportsController(
        IUnitOfWork unitOfWork,
        HMS.Infrastructure.Data.HmsDbContext context,
        ILogger<FinancialReportsController> logger)
    {
        _unitOfWork = unitOfWork;
        _context = context;
        _logger = logger;
    }

    [HttpGet("profit-loss")]
    public async Task<IActionResult> GetProfitLoss([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var start = startDate ?? DateTime.UtcNow.AddMonths(-1);
        var end = endDate ?? DateTime.UtcNow;

        var revenues = await _context.RevenueRecords
            .Where(r => r.RevenueDate >= start && r.RevenueDate <= end)
            .SumAsync(r => r.Amount);

        var expenses = await _context.Expenses
            .Where(e => e.ExpenseDate >= start && e.ExpenseDate <= end)
            .SumAsync(e => e.Amount);

        return Ok(new
        {
            PeriodStart = start,
            PeriodEnd = end,
            TotalRevenue = revenues,
            TotalExpenses = expenses,
            NetProfit = revenues - expenses,
            ProfitMargin = revenues > 0 ? ((revenues - expenses) / revenues * 100) : 0
        });
    }

    [HttpGet("balance-sheet")]
    public async Task<IActionResult> GetBalanceSheet([FromQuery] DateTime? date)
    {
        var asOfDate = date ?? DateTime.UtcNow;

        var totalAssets = await _context.Assets
            .Where(a => a.Status != "Retired" && a.Status != "Lost")
            .SumAsync(a => a.PurchasePrice ?? 0);

        var totalRevenue = await _context.RevenueRecords
            .Where(r => r.RevenueDate <= asOfDate)
            .SumAsync(r => r.Amount);

        var totalExpenses = await _context.Expenses
            .Where(e => e.ExpenseDate <= asOfDate)
            .SumAsync(e => e.Amount);

        var equity = totalRevenue - totalExpenses;

        return Ok(new
        {
            AsOfDate = asOfDate,
            Assets = new
            {
                Total = totalAssets
            },
            Liabilities = new
            {
                Total = 0 // Can be extended
            },
            Equity = equity,
            Total = totalAssets + equity
        });
    }

    [HttpGet("cash-flow")]
    public async Task<IActionResult> GetCashFlow([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var start = startDate ?? DateTime.UtcNow.AddMonths(-1);
        var end = endDate ?? DateTime.UtcNow;

        var cashIn = await _context.RevenueRecords
            .Where(r => r.RevenueDate >= start && r.RevenueDate <= end)
            .SumAsync(r => r.Amount);

        var cashOut = await _context.Expenses
            .Where(e => e.ExpenseDate >= start && e.ExpenseDate <= end)
            .SumAsync(e => e.Amount);

        return Ok(new
        {
            PeriodStart = start,
            PeriodEnd = end,
            CashIn = cashIn,
            CashOut = cashOut,
            NetCashFlow = cashIn - cashOut
        });
    }
}

