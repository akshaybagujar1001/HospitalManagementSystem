using AutoMapper;
using HMS.Application.DTOs.Appointment;
using HMS.Application.DTOs.Invoice;
using HMS.Application.DTOs.Patient;
using HMS.Domain.Entities;
using HMS.Domain.Interfaces;
using HMS.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PatientsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<PatientsController> _logger;
    private readonly HmsDbContext _context;

    public PatientsController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<PatientsController> logger, HmsDbContext context)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _context = context;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Doctor,Nurse")]
    public async Task<IActionResult> GetAll()
    {
        var patients = await _unitOfWork.Patients.GetAllAsync();
        var patientDtos = _mapper.Map<IEnumerable<PatientDto>>(patients);
        return Ok(patientDtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var patient = await _unitOfWork.Patients.GetByIdAsync(id);
        if (patient == null)
            return NotFound();

        // Check authorization - patients can only view their own records
        var userRole = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;
        var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        if (userRole == "Patient" && patient.UserId != userId)
            return Forbid();

        var patientDto = _mapper.Map<PatientDto>(patient);
        return Ok(patientDto);
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        try
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

            if (userId == 0)
            {
                return Unauthorized(new { message = "Invalid user token" });
            }

            // Load patient with User navigation property using Include
            var patient = await _context.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.UserId == userId);
            
            // If patient doesn't exist, create one automatically
            if (patient == null)
            {
                _logger.LogInformation("Patient profile not found for userId: {UserId}, creating new patient record", userId);
                
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                if (user.Role != "Patient")
                {
                    return BadRequest(new { message = "User is not a patient" });
                }

                // Generate a unique patient number
                var patientCount = await _context.Patients.CountAsync();
                var patientNumber = $"PAT-{DateTime.UtcNow:yyyyMMdd}-{(patientCount + 1):D3}";

                // Create new patient record
                patient = new Patient
                {
                    UserId = userId,
                    PatientNumber = patientNumber,
                    DateOfBirth = DateTime.UtcNow.AddYears(-30), // Default age
                    Gender = "Other", // MaxLength(10) - using "Other" instead of "Not Specified"
                    Address = string.Empty,
                    BloodGroup = null,
                    Allergies = null,
                    EmergencyContactName = null,
                    EmergencyContactPhone = null,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();

                // Reload with User navigation property
                patient = await _context.Patients
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.UserId == userId);
            }

            if (patient == null)
            {
                return StatusCode(500, new { message = "Failed to create patient profile" });
            }

            var patientDto = _mapper.Map<PatientDto>(patient);
            return Ok(patientDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading patient profile");
            return StatusCode(500, new { message = "An error occurred while loading patient profile" });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreatePatientDto createPatientDto)
    {
        try
        {
            // Create user first
            var user = new User
            {
                FirstName = createPatientDto.FirstName,
                LastName = createPatientDto.LastName,
                Email = createPatientDto.Email,
                PhoneNumber = createPatientDto.PhoneNumber,
                Role = "Patient",
                PasswordHash = "TemporaryPassword123!", // Should be set properly
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // Create patient
            var patient = new Patient
            {
                UserId = user.Id,
                PatientNumber = $"PAT-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}",
                DateOfBirth = createPatientDto.DateOfBirth,
                Gender = createPatientDto.Gender,
                Address = createPatientDto.Address,
                BloodGroup = createPatientDto.BloodGroup,
                Allergies = createPatientDto.Allergies,
                EmergencyContactName = createPatientDto.EmergencyContactName,
                EmergencyContactPhone = createPatientDto.EmergencyContactPhone,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Patients.AddAsync(patient);
            await _unitOfWork.SaveChangesAsync();

            var patientDto = _mapper.Map<PatientDto>(patient);
            return CreatedAtAction(nameof(GetById), new { id = patient.Id }, patientDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating patient");
            return StatusCode(500, new { message = "An error occurred while creating the patient" });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreatePatientDto updatePatientDto)
    {
        var patient = await _unitOfWork.Patients.GetByIdAsync(id);
        if (patient == null)
            return NotFound();

        // Check authorization
        var userRole = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;
        var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        if (userRole == "Patient" && patient.UserId != userId)
            return Forbid();

        var user = await _unitOfWork.Users.GetByIdAsync(patient.UserId);
        if (user != null)
        {
            user.FirstName = updatePatientDto.FirstName;
            user.LastName = updatePatientDto.LastName;
            user.Email = updatePatientDto.Email;
            user.PhoneNumber = updatePatientDto.PhoneNumber;
            user.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Users.UpdateAsync(user);
        }

        patient.DateOfBirth = updatePatientDto.DateOfBirth;
        patient.Gender = updatePatientDto.Gender;
        patient.Address = updatePatientDto.Address;
        patient.BloodGroup = updatePatientDto.BloodGroup;
        patient.Allergies = updatePatientDto.Allergies;
        patient.EmergencyContactName = updatePatientDto.EmergencyContactName;
        patient.EmergencyContactPhone = updatePatientDto.EmergencyContactPhone;
        patient.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Patients.UpdateAsync(patient);
        await _unitOfWork.SaveChangesAsync();

        var patientDto = _mapper.Map<PatientDto>(patient);
        return Ok(patientDto);
    }

    [HttpGet("{id}/appointments")]
    public async Task<IActionResult> GetAppointments(int id)
    {
        var patient = await _unitOfWork.Patients.GetByIdAsync(id);
        if (patient == null)
            return NotFound();

        // Check authorization
        var userRole = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;
        var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        if (userRole == "Patient" && patient.UserId != userId)
            return Forbid();

        var appointments = await _unitOfWork.Appointments.FindAsync(a => a.PatientId == id);
        var appointmentDtos = _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
        return Ok(appointmentDtos);
    }

    [HttpGet("{id}/medical-records")]
    public async Task<IActionResult> GetMedicalRecords(int id)
    {
        var patient = await _unitOfWork.Patients.GetByIdAsync(id);
        if (patient == null)
            return NotFound();

        // Check authorization
        var userRole = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;
        var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        if (userRole == "Patient" && patient.UserId != userId)
            return Forbid();

        var records = await _unitOfWork.MedicalRecords.FindAsync(r => r.PatientId == id);
        return Ok(records);
    }

    [HttpGet("{id}/invoices")]
    public async Task<IActionResult> GetInvoices(int id)
    {
        var patient = await _unitOfWork.Patients.GetByIdAsync(id);
        if (patient == null)
            return NotFound();

        // Check authorization
        var userRole = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;
        var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        if (userRole == "Patient" && patient.UserId != userId)
            return Forbid();

        var invoices = await _unitOfWork.Invoices.FindAsync(i => i.PatientId == id);
        var invoiceDtos = _mapper.Map<IEnumerable<InvoiceDto>>(invoices);
        return Ok(invoiceDtos);
    }
}

