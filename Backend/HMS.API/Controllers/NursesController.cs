using AutoMapper;
using HMS.Application.DTOs.Doctor;
using HMS.Domain.Entities;
using HMS.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NursesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<NursesController> _logger;

    public NursesController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<NursesController> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? department)
    {
        IEnumerable<Nurse> nurses;
        
        if (!string.IsNullOrEmpty(department))
        {
            nurses = await _unitOfWork.Nurses.FindAsync(n => n.Department == department && n.IsAvailable);
        }
        else
        {
            nurses = await _unitOfWork.Nurses.FindAsync(n => n.IsAvailable);
        }

        return Ok(nurses);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var nurse = await _unitOfWork.Nurses.GetByIdAsync(id);
        if (nurse == null)
            return NotFound();

        return Ok(nurse);
    }
}

