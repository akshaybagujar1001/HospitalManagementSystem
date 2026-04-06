using HMS.Domain.Entities;

namespace HMS.Domain.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
    string? ValidateToken(string token);
}

