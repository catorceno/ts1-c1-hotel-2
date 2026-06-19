public interface IBookingService
{
    Task<BookingResponseDto> CreateBooking(CreateBookingDto bookingDto);
    Task<BookingStatusResponseDto> CheckIn(long id);
    Task<BookingStatusResponseDto> CheckOut(long id);
    Task<List<BookingDetailsReponseDto>> GetBookings();
}