public class CreateBookingDto
{
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public long RoomId { get; set; }
    public List<CreateGuestDto> Guests { get; set; }
}