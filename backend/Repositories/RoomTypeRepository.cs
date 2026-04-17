public class RoomTypeRepository : IRoomTypeRepository
{
    private readonly Supabase.Client _client;
    public RoomTypeRepository(Supabase.Client client)
    {
        _client = client;
    }

    public async Task<RoomType> GetByIdAsync(long id)
    {
        var response = await _client.From<RoomType>().Where(r => r.Id == id).Single();
        return response;
    }

    public async Task<List<RoomType>> GetAllAsync()
    {
        var response = await _client.From<RoomType>().Get();
        return response.Models;
    }
}