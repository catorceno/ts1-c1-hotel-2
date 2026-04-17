public interface IRoomRepository
{
    Task<Room> GetByIdAsync(long id);
    Task<List<Room>> GetAllAsync();
}