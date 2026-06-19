public interface IRoomTypeRepository
{
    Task<RoomType> GetByIdAsync(long id);
    Task<List<RoomType>> GetAllAsync();
}