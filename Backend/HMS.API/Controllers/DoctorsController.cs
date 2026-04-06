using AutoMapper;
using HMS.Application.DTOs.Appointment;
using HMS.Application.DTOs.Doctor;
using HMS.Domain.Entities;
using HMS.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace HMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DoctorsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<DoctorsController> _logger;

    public DoctorsController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<DoctorsController> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? specialization)
    {
        IEnumerable<Doctor> doctors;
        
        if (!string.IsNullOrEmpty(specialization))
        {
            doctors = await _unitOfWork.Doctors.FindAsync(d => d.Specialization == specialization && d.IsAvailable);
        }
        else
        {
            doctors = await _unitOfWork.Doctors.FindAsync(d => d.IsAvailable);
        }

        var doctorDtos = _mapper.Map<IEnumerable<DoctorDto>>(doctors);
        return Ok(doctorDtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var doctor = await _unitOfWork.Doctors.GetByIdAsync(id);
        if (doctor == null)
            return NotFound();

        var doctorDto = _mapper.Map<DoctorDto>(doctor);
        return Ok(doctorDto);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateDoctorDto createDoctorDto)
    {
        try
        {
            // Validate password is provided for create
            if (string.IsNullOrEmpty(createDoctorDto.Password))
            {
                return BadRequest(new { message = "Password is required" });
            }

            // Check if email already exists
            var existingUser = (await _unitOfWork.Users.FindAsync(u => u.Email == createDoctorDto.Email)).FirstOrDefault();
            if (existingUser != null)
            {
                return BadRequest(new { message = "Email already exists" });
            }

            // Create user first
            var user = new User
            {
                FirstName = createDoctorDto.FirstName,
                LastName = createDoctorDto.LastName,
                Email = createDoctorDto.Email,
                PhoneNumber = createDoctorDto.PhoneNumber,
                Role = "Doctor",
                PasswordHash = HashPassword(createDoctorDto.Password!),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // Create doctor
            var doctor = new Doctor
            {
                UserId = user.Id,
                DoctorNumber = $"DOC-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}",
                Specialization = createDoctorDto.Specialization,
                LicenseNumber = createDoctorDto.LicenseNumber,
                PhoneNumber = createDoctorDto.PhoneNumber,
                Bio = createDoctorDto.Bio,
                ConsultationFee = createDoctorDto.ConsultationFee,
                IsAvailable = createDoctorDto.IsAvailable,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Doctors.AddAsync(doctor);
            await _unitOfWork.SaveChangesAsync();

            var doctorDto = _mapper.Map<DoctorDto>(doctor);
            return CreatedAtAction(nameof(GetById), new { id = doctor.Id }, doctorDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating doctor");
            return StatusCode(500, new { message = "An error occurred while creating the doctor" });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateDoctorDto updateDoctorDto)
    {
        try
        {
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(id);
            if (doctor == null)
                return NotFound();

            var user = await _unitOfWork.Users.GetByIdAsync(doctor.UserId);
            if (user != null)
            {
                // Check if email is being changed and if it already exists
                if (user.Email != updateDoctorDto.Email)
                {
                    var existingUser = (await _unitOfWork.Users.FindAsync(u => u.Email == updateDoctorDto.Email && u.Id != user.Id)).FirstOrDefault();
                    if (existingUser != null)
                    {
                        return BadRequest(new { message = "Email already exists" });
                    }
                }

                user.FirstName = updateDoctorDto.FirstName;
                user.LastName = updateDoctorDto.LastName;
                user.Email = updateDoctorDto.Email;
                user.PhoneNumber = updateDoctorDto.PhoneNumber;
                user.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Users.UpdateAsync(user);
            }

            doctor.Specialization = updateDoctorDto.Specialization;
            doctor.LicenseNumber = updateDoctorDto.LicenseNumber;
            doctor.PhoneNumber = updateDoctorDto.PhoneNumber;
            doctor.Bio = updateDoctorDto.Bio;
            doctor.ConsultationFee = updateDoctorDto.ConsultationFee;
            doctor.IsAvailable = updateDoctorDto.IsAvailable;
            doctor.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Doctors.UpdateAsync(doctor);
            await _unitOfWork.SaveChangesAsync();

            var doctorDto = _mapper.Map<DoctorDto>(doctor);
            return Ok(doctorDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating doctor");
            return StatusCode(500, new { message = "An error occurred while updating the doctor" });
        }
    }

    [HttpGet("{id}/appointments")]
    public async Task<IActionResult> GetAppointments(int id)
    {
        var doctor = await _unitOfWork.Doctors.GetByIdAsync(id);
        if (doctor == null)
            return NotFound();

        var appointments = await _unitOfWork.Appointments.FindAsync(a => a.DoctorId == id);
        var appointmentDtos = _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
        return Ok(appointmentDtos);
    }

    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}

