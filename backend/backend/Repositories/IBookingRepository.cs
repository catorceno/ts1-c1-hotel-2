public interface IBookingRepository
{
    Task<Booking> CreateAsync(Booking booking);
    Task<List<Booking>> GetAllAsync();
    Task<List<Booking>> GetByRoomIdAsync(long roomId);
    Task<Booking> GetByIdAsync(long id);
    Task<Booking> UpdateAsync(Booking booking);
}