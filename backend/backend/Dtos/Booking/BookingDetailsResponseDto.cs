public class BookingDetailsReponseDto
{
    public long Id { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public DateTimeOffset? CheckIn { get; set; }
    public DateTimeOffset? CheckOut { get; set; }
    public BookingStatus? Status { get; set; }
    public RoomResponseDto Room { get; set; }
    public PaymentResponseDto Payment { get; set; }
    public List<GuestResponseDto> Guests { get; set; }
}