using HMS.Application.DTOs.Auth;
using HMS.Application.Services;
using HMS.Application.Validators;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace HMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        try
        {
            if (registerDto == null)
            {
                return BadRequest(new { message = "Request body is required" });
            }

            var validator = new RegisterDtoValidator();
            var validationResult = await validator.ValidateAsync(registerDto);
            
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => new { 
                    field = e.PropertyName, 
                    message = e.ErrorMessage 
                });
                return BadRequest(new { message = "Validation failed", errors });
            }

            var result = await _authService.RegisterAsync(registerDto);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Registration failed: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration: {Error}", ex.ToString());
            return StatusCode(500, new { message = "An error occurred during registration", details = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            if (loginDto == null)
            {
                return BadRequest(new { message = "Request body is required" });
            }

            var result = await _authService.LoginAsync(loginDto);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Login failed: {Message}", ex.Message);
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login: {Error}", ex.ToString());
            return StatusCode(500, new { message = "An error occurred during login", details = ex.Message });
        }
    }
}

