public class RoomRepository : IRoomRepository
{
    private readonly Supabase.Client _client;
    public RoomRepository(Supabase.Client client)
    {
        _client = client;
    }

    public async Task<Room> GetByIdAsync(long id)
    {
        var response = await _client.From<Room>().Where(r => r.Id == id).Single();
        return response;
    }

    public async Task<List<Room>> GetAllAsync()
    {
        var response = await _client.From<Room>().Get();
        return response.Models;
    }

}
