public interface IGuestBookingRepository
{
    Task<GuestBooking> CreateAsync(GuestBooking booking);
    Task<List<GuestBooking>> GetByBookingIdAsync(long bookingId);
}