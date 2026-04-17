public interface IRoomService
{
    Task<RoomResponseDto> GetDetailsById(long id);
    Task<bool> IsAvailable(long id, DateOnly startDate, DateOnly endDate);
    Task<List<AvailableRoomsDto>> GetAvailableRoomsByType(DateOnly startDate, DateOnly endDate);
}