using HMS.Domain.Entities;
using HMS.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RoomsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RoomsController> _logger;

    public RoomsController(IUnitOfWork unitOfWork, ILogger<RoomsController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? roomType, [FromQuery] bool? isAvailable)
    {
        IEnumerable<Room> rooms = await _unitOfWork.Rooms.GetAllAsync();

        if (!string.IsNullOrEmpty(roomType))
        {
            rooms = rooms.Where(r => r.RoomType == roomType);
        }

        if (isAvailable.HasValue)
        {
            rooms = rooms.Where(r => r.IsAvailable == isAvailable.Value);
        }

        return Ok(rooms);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var room = await _unitOfWork.Rooms.GetByIdAsync(id);
        if (room == null)
            return NotFound();

        return Ok(room);
    }

    [HttpGet("{id}/beds")]
    public async Task<IActionResult> GetBeds(int id)
    {
        var room = await _unitOfWork.Rooms.GetByIdAsync(id);
        if (room == null)
            return NotFound();

        var beds = await _unitOfWork.Beds.FindAsync(b => b.RoomId == id);
        return Ok(beds);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateRoomDto createDto)
    {
        try
        {
            var room = new Room
            {
                RoomNumber = createDto.RoomNumber,
                RoomType = createDto.RoomType,
                Floor = createDto.Floor,
                Description = createDto.Description,
                PricePerDay = createDto.PricePerDay,
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Rooms.AddAsync(room);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = room.Id }, room);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating room");
            return StatusCode(500, new { message = "An error occurred while creating the room" });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateRoomDto updateDto)
    {
        var room = await _unitOfWork.Rooms.GetByIdAsync(id);
        if (room == null)
            return NotFound();

        room.RoomNumber = updateDto.RoomNumber;
        room.RoomType = updateDto.RoomType;
        room.Floor = updateDto.Floor;
        room.Description = updateDto.Description;
        room.PricePerDay = updateDto.PricePerDay;
        room.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Rooms.UpdateAsync(room);
        await _unitOfWork.SaveChangesAsync();

        return Ok(room);
    }

    [HttpPost("{id}/beds")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddBed(int id, [FromBody] CreateBedDto createDto)
    {
        var room = await _unitOfWork.Rooms.GetByIdAsync(id);
        if (room == null)
            return NotFound();

        var bed = new Bed
        {
            RoomId = id,
            BedNumber = createDto.BedNumber,
            Description = createDto.Description,
            IsOccupied = false,
            IsAvailable = true,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Beds.AddAsync(bed);
        await _unitOfWork.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBeds), new { id }, bed);
    }
}

public class CreateRoomDto
{
    public string RoomNumber { get; set; } = string.Empty;
    public string RoomType { get; set; } = string.Empty;
    public string? Floor { get; set; }
    public string? Description { get; set; }
    public decimal PricePerDay { get; set; }
}

public class CreateBedDto
{
    public string BedNumber { get; set; } = string.Empty;
    public string? Description { get; set; }
}

