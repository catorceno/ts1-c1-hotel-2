public class GuestBookingRepository : IGuestBookingRepository
{
    private readonly Supabase.Client _client;
    public GuestBookingRepository(Supabase.Client client)
    {
        _client = client;
    }

    public async Task<GuestBooking> CreateAsync(GuestBooking guestBooking)
    {
        var response = await _client.From<GuestBooking>().Insert(guestBooking);
        return response.Models.First();
    }

    public async Task<List<GuestBooking>> GetByBookingIdAsync(long bookingId)
    {
        var response = await _client.From<GuestBooking>().Where(gb => gb.BookingId == bookingId).Get();
        return response.Models;
    }
}