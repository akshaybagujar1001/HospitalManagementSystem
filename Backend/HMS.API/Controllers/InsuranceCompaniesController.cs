using HMS.Application.DTOs.Insurance;
using HMS.Domain.Entities;
using HMS.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace HMS.API.Controllers;

[ApiController]
[Route("api/insurance-companies")]
[Authorize(Roles = "Admin,Finance")]
public class InsuranceCompaniesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<InsuranceCompaniesController> _logger;

    public InsuranceCompaniesController(IUnitOfWork unitOfWork, ILogger<InsuranceCompaniesController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool? activeOnly = false)
    {
        try
        {
            var companies = await _unitOfWork.InsuranceCompanies.FindAsync(c =>
                !activeOnly.HasValue || !activeOnly.Value || c.IsActive);

            var companyDtos = companies.Select(c => new InsuranceCompanyDto
            {
                Id = c.Id,
                Name = c.Name,
                ContactInfo = c.ContactInfo,
                PhoneNumber = c.PhoneNumber,
                Email = c.Email,
                Address = c.Address,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt
            });

            return Ok(companyDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting insurance companies: {Error}", ex.Message);
            return StatusCode(500, new { message = "An error occurred while retrieving insurance companies", details = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var company = await _unitOfWork.InsuranceCompanies.GetByIdAsync(id);
        if (company == null)
            return NotFound();

        var companyDto = new InsuranceCompanyDto
        {
            Id = company.Id,
            Name = company.Name,
            ContactInfo = company.ContactInfo,
            PhoneNumber = company.PhoneNumber,
            Email = company.Email,
            Address = company.Address,
            PolicyRules = company.PolicyRules,
            IsActive = company.IsActive,
            CreatedAt = company.CreatedAt
        };

        return Ok(companyDto);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateInsuranceCompanyDto createDto)
    {
        try
        {
            if (createDto == null)
            {
                return BadRequest(new { message = "Request body is required" });
            }

            if (string.IsNullOrWhiteSpace(createDto.Name))
            {
                return BadRequest(new { message = "Company name is required" });
            }

            var company = new InsuranceCompany
            {
                Name = createDto.Name.Trim(),
                ContactInfo = createDto.ContactInfo?.Trim(),
                PhoneNumber = createDto.PhoneNumber?.Trim(),
                Email = createDto.Email?.Trim(),
                Address = createDto.Address?.Trim(),
                PolicyRules = createDto.PolicyRules?.Trim(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.InsuranceCompanies.AddAsync(company);
            await _unitOfWork.SaveChangesAsync();

            var companyDto = new InsuranceCompanyDto
            {
                Id = company.Id,
                Name = company.Name,
                ContactInfo = company.ContactInfo,
                PhoneNumber = company.PhoneNumber,
                Email = company.Email,
                Address = company.Address,
                PolicyRules = company.PolicyRules,
                IsActive = company.IsActive,
                CreatedAt = company.CreatedAt
            };

            return CreatedAtAction(nameof(GetById), new { id = company.Id }, companyDto);
        }
        catch (DbUpdateException dbEx)
        {
            // Log FULL exception details
            _logger.LogError(dbEx, "=== DATABASE ERROR CREATING INSURANCE COMPANY ===");
            _logger.LogError("Exception Type: {Type}", dbEx.GetType().FullName);
            _logger.LogError("Exception Message: {Message}", dbEx.Message);
            _logger.LogError("Stack Trace: {StackTrace}", dbEx.StackTrace);
            
            // Log inner exception details
            var innerEx = dbEx.InnerException;
            int depth = 0;
            while (innerEx != null)
            {
                depth++;
                _logger.LogError("Inner Exception [{Depth}]: {Type} - {Message}", depth, innerEx.GetType().FullName, innerEx.Message);
                _logger.LogError("Inner Exception [{Depth}] Stack Trace: {StackTrace}", depth, innerEx.StackTrace);
                
                // Check for SqlException
                if (innerEx is Microsoft.Data.SqlClient.SqlException sqlEx)
                {
                    _logger.LogError("SQL Exception Number: {Number}", sqlEx.Number);
                    _logger.LogError("SQL Exception State: {State}", sqlEx.State);
                    _logger.LogError("SQL Exception Class: {Class}", sqlEx.Class);
                    _logger.LogError("SQL Exception Server: {Server}", sqlEx.Server);
                    _logger.LogError("SQL Exception Procedure: {Procedure}", sqlEx.Procedure);
                    _logger.LogError("SQL Exception LineNumber: {LineNumber}", sqlEx.LineNumber);
                    _logger.LogError("SQL Errors Count: {Count}", sqlEx.Errors.Count);
                    for (int i = 0; i < sqlEx.Errors.Count; i++)
                    {
                        var error = sqlEx.Errors[i];
                        _logger.LogError("SQL Error [{Index}]: Number={Number}, State={State}, Class={Class}, Message={Message}, Source={Source}", 
                            i, error.Number, error.State, error.Class, error.Message, error.Source);
                    }
                }
                
                innerEx = innerEx.InnerException;
            }
            
            // Log entity state if available
            if (dbEx.Entries != null && dbEx.Entries.Any())
            {
                foreach (var entry in dbEx.Entries)
                {
                    _logger.LogError("Entity: {EntityType}, State: {State}", entry.Entity.GetType().Name, entry.State);
                    foreach (var prop in entry.Properties)
                    {
                        _logger.LogError("  Property: {Property} = {Value} (IsModified: {IsModified}, IsTemporary: {IsTemporary})", 
                            prop.Metadata.Name, prop.CurrentValue, prop.IsModified, prop.IsTemporary);
                    }
                }
            }
            
            _logger.LogError("=== END DATABASE ERROR ===");
            
            // Return detailed error
            var errorDetails = new
            {
                message = "Database error occurred",
                exceptionType = dbEx.GetType().FullName,
                exceptionMessage = dbEx.Message,
                stackTrace = dbEx.StackTrace,
                innerException = dbEx.InnerException?.Message,
                sqlException = dbEx.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx2 
                    ? new
                    {
                        number = sqlEx2.Number,
                        state = sqlEx2.State,
                        class_ = sqlEx2.Class,
                        message = sqlEx2.Message,
                        server = sqlEx2.Server,
                        errors = sqlEx2.Errors.Cast<Microsoft.Data.SqlClient.SqlError>()
                            .Select(e => new { number = e.Number, state = e.State, message = e.Message, source = e.Source })
                            .ToList()
                    }
                    : null
            };
            
            return StatusCode(500, errorDetails);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "=== GENERAL ERROR CREATING INSURANCE COMPANY ===");
            _logger.LogError("Exception Type: {Type}", ex.GetType().FullName);
            _logger.LogError("Exception Message: {Message}", ex.Message);
            _logger.LogError("Stack Trace: {StackTrace}", ex.StackTrace);
            
            var innerEx = ex.InnerException;
            int depth = 0;
            while (innerEx != null)
            {
                depth++;
                _logger.LogError("Inner Exception [{Depth}]: {Type} - {Message}", depth, innerEx.GetType().FullName, innerEx.Message);
                _logger.LogError("Inner Exception [{Depth}] Stack Trace: {StackTrace}", depth, innerEx.StackTrace);
                innerEx = innerEx.InnerException;
            }
            _logger.LogError("=== END GENERAL ERROR ===");
            
            return StatusCode(500, new 
            { 
                message = "An error occurred while creating the insurance company",
                exceptionType = ex.GetType().FullName,
                exceptionMessage = ex.Message,
                stackTrace = ex.StackTrace,
                innerException = ex.InnerException?.Message
            });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateInsuranceCompanyDto updateDto)
    {
        var company = await _unitOfWork.InsuranceCompanies.GetByIdAsync(id);
        if (company == null)
            return NotFound();

        company.Name = updateDto.Name;
        company.ContactInfo = updateDto.ContactInfo;
        company.PhoneNumber = updateDto.PhoneNumber;
        company.Email = updateDto.Email;
        company.Address = updateDto.Address;
        company.PolicyRules = updateDto.PolicyRules;
        company.IsActive = updateDto.IsActive;
        company.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.InsuranceCompanies.UpdateAsync(company);
        await _unitOfWork.SaveChangesAsync();

        return Ok(company);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var company = await _unitOfWork.InsuranceCompanies.GetByIdAsync(id);
        if (company == null)
            return NotFound();

        // Check if company has active policies
        var hasPolicies = await _unitOfWork.InsurancePolicies.ExistsAsync(p => p.CompanyId == id && p.IsActive);
        if (hasPolicies)
            return BadRequest(new { message = "Cannot delete company with active policies" });

        await _unitOfWork.InsuranceCompanies.DeleteAsync(company);
        await _unitOfWork.SaveChangesAsync();

        return Ok(new { message = "Insurance company deleted" });
    }
}

