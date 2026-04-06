using AutoMapper;
using HMS.Application.DTOs.Invoice;
using HMS.Domain.Entities;
using HMS.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InvoicesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPdfService _pdfService;
    private readonly ILogger<InvoicesController> _logger;

    public InvoicesController(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IPdfService pdfService,
        ILogger<InvoicesController> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _pdfService = pdfService;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var invoices = await _unitOfWork.Invoices.GetAllAsync();
        var invoiceDtos = _mapper.Map<IEnumerable<InvoiceDto>>(invoices);
        return Ok(invoiceDtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var invoice = await _unitOfWork.Invoices.GetByIdAsync(id);
        if (invoice == null)
            return NotFound();

        // Check authorization
        var userRole = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;
        var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        if (userRole == "Patient")
        {
            var patient = await _unitOfWork.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient == null || invoice.PatientId != patient.Id)
                return Forbid();
        }

        var invoiceDto = _mapper.Map<InvoiceDto>(invoice);
        return Ok(invoiceDto);
    }

    [HttpGet("{id}/pdf")]
    public async Task<IActionResult> GetPdf(int id)
    {
        var invoice = await _unitOfWork.Invoices.GetByIdAsync(id);
        if (invoice == null)
            return NotFound();

        // Check authorization
        var userRole = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;
        var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        if (userRole == "Patient")
        {
            var patient = await _unitOfWork.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient == null || invoice.PatientId != patient.Id)
                return Forbid();
        }

        var pdfBytes = await _pdfService.GenerateInvoicePdfAsync(id);
        return File(pdfBytes, "application/pdf", $"Invoice-{invoice.InvoiceNumber}.pdf");
    }

    [HttpPut("{id}/pay")]
    public async Task<IActionResult> MarkAsPaid(int id)
    {
        var invoice = await _unitOfWork.Invoices.GetByIdAsync(id);
        if (invoice == null)
            return NotFound();

        invoice.Status = "Paid";
        invoice.PaidDate = DateTime.UtcNow;
        invoice.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Invoices.UpdateAsync(invoice);
        await _unitOfWork.SaveChangesAsync();

        return Ok(new { message = "Invoice marked as paid" });
    }
}

