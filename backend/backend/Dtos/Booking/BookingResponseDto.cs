public class BookingResponseDto
{
    public long Id { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public BookingStatus Status { get; set; }
    public RoomResponseDto Room { get; set; }
    public List<GuestResponseDto> Guests { get; set; }
}