using AutoMapper;
using HMS.Application.DTOs.Auth;
using HMS.Domain.Entities;
using HMS.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace HMS.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;

    public AuthService(
        IUnitOfWork unitOfWork,
        IJwtService jwtService,
        IMapper mapper,
        IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
        _mapper = mapper;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        if (await UserExistsAsync(registerDto.Email))
        {
            throw new InvalidOperationException("User with this email already exists");
        }

        var user = _mapper.Map<User>(registerDto);
        user.PasswordHash = HashPassword(registerDto.Password);
        user.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        // Create role-specific entity (Admin doesn't need a separate entity)
        if (registerDto.Role == "Patient")
        {
            var patient = new Patient
            {
                UserId = user.Id,
                PatientNumber = GeneratePatientNumber(),
                DateOfBirth = DateTime.UtcNow.AddYears(-30), // Default, should be provided
                Gender = "Unknown",
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Patients.AddAsync(patient);
            await _unitOfWork.SaveChangesAsync();
        }
        else if (registerDto.Role == "Doctor")
        {
            var doctor = new Doctor
            {
                UserId = user.Id,
                DoctorNumber = GenerateDoctorNumber(),
                Specialization = "General",
                ConsultationFee = 0,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Doctors.AddAsync(doctor);
            await _unitOfWork.SaveChangesAsync();
        }
        else if (registerDto.Role == "Nurse")
        {
            var nurse = new Nurse
            {
                UserId = user.Id,
                NurseNumber = GenerateNurseNumber(),
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Nurses.AddAsync(nurse);
            await _unitOfWork.SaveChangesAsync();
        }
        // Admin role doesn't need a separate entity

        var response = _mapper.Map<AuthResponseDto>(user);
        response.Token = _jwtService.GenerateToken(user);
        
        return response;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
        
        if (user == null || !VerifyPassword(loginDto.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("User account is inactive");
        }

        var response = _mapper.Map<AuthResponseDto>(user);
        response.Token = _jwtService.GenerateToken(user);
        
        return response;
    }

    public async Task<bool> UserExistsAsync(string email)
    {
        return await _unitOfWork.Users.ExistsAsync(u => u.Email == email);
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private bool VerifyPassword(string password, string hash)
    {
        var hashOfInput = HashPassword(password);
        return hashOfInput == hash;
    }

    private string GeneratePatientNumber()
    {
        return $"PAT-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}";
    }

    private string GenerateDoctorNumber()
    {
        return $"DOC-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}";
    }

    private string GenerateNurseNumber()
    {
        return $"NUR-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}";
    }
}

