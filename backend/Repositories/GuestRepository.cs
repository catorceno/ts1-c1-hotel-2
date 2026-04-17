public class GuestRepository : IGuestRepository
{
    private readonly Supabase.Client _client;
    public GuestRepository(Supabase.Client client)
    {
        _client = client;
    }

    public async Task<Guest> CreateAsync(Guest guest)
    {
        var response = await _client.From<Guest>().Insert(guest);
        return response.Models.First();
    }

    public async Task<List<Guest>> GetAllAsync()
    {
        var response = await _client.From<Guest>().Get();
        return response.Models;
    }

    public async Task<Guest> GetByIdAsync(long id)
    {
        var response = await _client.From<Guest>().Where(g => g.Id == id).Single();
        return response;
    }
}