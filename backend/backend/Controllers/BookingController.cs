using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;
    public BookingController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpPost]
    public async Task<ActionResult<BookingResponseDto>> CreateBooking(CreateBookingDto bookingDto)
    {
        try
        {
            var booking = await _bookingService.CreateBooking(bookingDto);
            return Ok(booking);
        }
        catch(Exception e)
        {
            return BadRequest(new { message = e.Message });
        }
    }

    [HttpPost("{id}/checkin")]
    public async Task<ActionResult<BookingStatusResponseDto>> CheckIn(long id)
    {
        try
        {
            var booking = await _bookingService.CheckIn(id);
            return Ok(booking);
        }
        catch(Exception e)
        {
            return BadRequest(new { message = e.Message });
        }
    }

    [HttpPost("{id}/checkout")]
    public async Task<ActionResult<BookingStatusResponseDto>> CheckOut(long id)
    {
        try
        {
            var booking = await _bookingService.CheckOut(id);
            return Ok(booking);
        }
        catch(Exception e)
        {
            return BadRequest(new { message = e.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<BookingDetailsReponseDto>>> GetBookings()
    {
        try
        {
            var bookings = await _bookingService.GetBookings();
            return Ok(bookings);
        }
        catch(Exception e)
        {
            return BadRequest(new { message = e.Message });
        }
    }
}

/*
{
  "startDate": "2026-04-17",
  "endDate": "2026-05-17",
  "roomId": 50,
  "guests": [
    {
      "name": "prueba prueba prueba 1",
      "ci": "1234567",
      "phone": "12345678"
    }
  ]
}
*/