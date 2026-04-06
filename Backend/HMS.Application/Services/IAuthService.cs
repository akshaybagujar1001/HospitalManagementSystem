using HMS.Application.DTOs.Auth;

namespace HMS.Application.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    Task<bool> UserExistsAsync(string email);
}

