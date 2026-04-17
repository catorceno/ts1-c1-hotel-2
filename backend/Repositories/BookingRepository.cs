public class BookingRepository : IBookingRepository
{
    private readonly Supabase.Client _client;
    public BookingRepository(Supabase.Client client)
    {
        _client = client;
    }

    public async Task<Booking> CreateAsync(Booking booking)
    {
        var response = await _client.From<Booking>().Insert(booking);
        return response.Models.First();
    }

    public async Task<List<Booking>> GetAllAsync()
    {
        var reponse = await _client.From<Booking>().Order(b => b.StartDate, Postgrest.Constants.Ordering.Descending).Get();
        return reponse.Models;
    }

    public async Task<List<Booking>> GetByRoomIdAsync(long roomId)
    {
        var response = await _client.From<Booking>().Where(b => b.RoomId == roomId).Get();
        return response.Models;
    }

    public async Task<Booking> GetByIdAsync(long id)
    {
        var response = await _client.From<Booking>().Where(b => b.Id == id).Single();
        return response;
    }

    public async Task<Booking> UpdateAsync(Booking booking)
    {
        // validar nulls?
        var response = await _client.From<Booking>().Update(booking);
        return response.Models.FirstOrDefault();
    }
}