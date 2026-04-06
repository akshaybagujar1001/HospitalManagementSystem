using HMS.Application.DTOs.Finance;
using HMS.Domain.Entities;
using HMS.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HMS.API.Controllers;

[ApiController]
[Route("api/expenses")]
[Authorize(Roles = "Admin,Finance")]
public class ExpensesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ExpensesController> _logger;

    public ExpensesController(IUnitOfWork unitOfWork, ILogger<ExpensesController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? category, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var expenses = await _unitOfWork.Expenses.FindAsync(e =>
            (category == null || e.Category == category) &&
            (!startDate.HasValue || e.ExpenseDate >= startDate.Value) &&
            (!endDate.HasValue || e.ExpenseDate <= endDate.Value));

        var expenseDtos = expenses.Select(e => new ExpenseDto
        {
            Id = e.Id,
            Category = e.Category,
            Amount = e.Amount,
            Description = e.Description,
            Vendor = e.Vendor,
            PaymentMethod = e.PaymentMethod,
            ExpenseDate = e.ExpenseDate,
            CreatedAt = e.CreatedAt
        });

        return Ok(expenseDtos);
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var start = startDate ?? DateTime.UtcNow.AddMonths(-1);
        var end = endDate ?? DateTime.UtcNow;

        var expenses = await _unitOfWork.Expenses.FindAsync(e =>
            e.ExpenseDate >= start && e.ExpenseDate <= end);

        var total = expenses.Sum(e => e.Amount);
        var byCategory = expenses.GroupBy(e => e.Category)
            .Select(g => new { Category = g.Key, Total = g.Sum(e => e.Amount) });

        return Ok(new
        {
            Total = total,
            ByCategory = byCategory,
            PeriodStart = start,
            PeriodEnd = end
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var expense = await _unitOfWork.Expenses.GetByIdAsync(id);
        if (expense == null)
            return NotFound();

        return Ok(expense);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateExpenseDto createDto)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var expense = new Expense
            {
                Category = createDto.Category,
                Amount = createDto.Amount,
                Description = createDto.Description,
                Vendor = createDto.Vendor,
                PaymentMethod = createDto.PaymentMethod,
                ExpenseDate = createDto.ExpenseDate ?? DateTime.UtcNow,
                CreatedByUserId = userId,
                Notes = createDto.Notes,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Expenses.AddAsync(expense);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = expense.Id }, expense);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating expense");
            return StatusCode(500, new { message = "An error occurred while creating the expense" });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateExpenseDto updateDto)
    {
        var expense = await _unitOfWork.Expenses.GetByIdAsync(id);
        if (expense == null)
            return NotFound();

        expense.Category = updateDto.Category;
        expense.Amount = updateDto.Amount;
        expense.Description = updateDto.Description;
        expense.Vendor = updateDto.Vendor;
        expense.PaymentMethod = updateDto.PaymentMethod;
        expense.ExpenseDate = updateDto.ExpenseDate;
        expense.Notes = updateDto.Notes;

        await _unitOfWork.Expenses.UpdateAsync(expense);
        await _unitOfWork.SaveChangesAsync();

        return Ok(expense);
    }
}

