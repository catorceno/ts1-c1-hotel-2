public interface IGuestRepository
{
    Task<Guest> CreateAsync(Guest guest);
    Task<List<Guest>> GetAllAsync();
    Task<Guest> GetByIdAsync(long id);
}