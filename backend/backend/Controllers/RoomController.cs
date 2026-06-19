using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class RoomController : ControllerBase
{
    private readonly IRoomService _roomService;

    public RoomController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    [HttpGet("available")]
    public async Task<ActionResult<List<AvailableRoomsDto>>> GetAvailableRooms(
        [FromQuery] DateOnly startDate,
        [FromQuery] DateOnly endDate)
    {
        try
        {
            var availableRooms = await _roomService.GetAvailableRoomsByType(startDate, endDate);
            return Ok(availableRooms);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message });
        }
    }
}

/*
GET /api/room/available?startDate=2026-04-17&endDate=2026-05-17
*/
