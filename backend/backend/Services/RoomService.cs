using AutoMapper;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepo;
    private readonly IBookingRepository _bookingRepo;
    private readonly IRoomTypeRepository _roomTypeRepo;
    private readonly IMapper _mapper;

    public RoomService(
        IRoomRepository roomRepository, 
        IBookingRepository bookingRepository,
        IRoomTypeRepository roomTypeRepository,
        IMapper mapper)
    {
        _roomRepo = roomRepository;
        _bookingRepo = bookingRepository;
        _roomTypeRepo = roomTypeRepository;
        _mapper = mapper;
    }

    public async Task<bool> IsAvailable(long id, DateOnly startDate, DateOnly endDate)
    {
        if (startDate >= endDate)
            throw new ArgumentException("La fecha de inicio debe ser anterior a la fecha de fin.");
        
        var bookings = await _bookingRepo.GetByRoomIdAsync(id);
        var hasOverlap = bookings.Any(b => b.StartDate < endDate && b.EndDate > startDate); // Any --> true si hay alguno, false si no hay
        return !hasOverlap;
    }

    public async Task<RoomResponseDto> GetDetailsById(long id)
    {
        var room = await _roomRepo.GetByIdAsync(id);
        room.RoomType = await _roomTypeRepo.GetByIdAsync(room.TypeId);
        var roomResponseDto = _mapper.Map<RoomResponseDto>(room);
        return roomResponseDto;
    }

    public async Task<List<AvailableRoomsDto>> GetAvailableRoomsByType(DateOnly startDate, DateOnly endDate)
    {
        if (startDate >= endDate)
            throw new ArgumentException("La fecha de inicio debe ser anterior a la fecha de fin.");

        var allRooms = await _roomRepo.GetAllAsync();
        var allRoomTypes = await _roomTypeRepo.GetAllAsync();
        var allBookings = await _bookingRepo.GetAllAsync();
        
        var availabilityByType = new List<AvailableRoomsDto>(); // Agrupar habitaciones por tipo

        foreach (var roomType in allRoomTypes)
        {
            var roomsOfType = allRooms.Where(r => r.TypeId == roomType.Id).ToList();
            var availableRoomNumbers = new List<AvailableRoomItemDto>();

            foreach (var room in roomsOfType)
            {
                // Verificar si la habitación está disponible en el rango de fechas
                var roomBookings = allBookings.Where(b => b.RoomId == room.Id).ToList();
                var hasOverlap = roomBookings.Any(b => b.StartDate < endDate && b.EndDate > startDate);

                if (!hasOverlap)
                {
                    availableRoomNumbers.Add(new AvailableRoomItemDto
                    {
                        Id = room.Id,
                        Number = room.Number
                    });
                }
            }

            // Solo agregar el tipo si hay habitaciones disponibles
            if (availableRoomNumbers.Count > 0)
            {
                availabilityByType.Add(new AvailableRoomsDto
                {
                    RoomType = roomType.Name,
                    AvailableRooms = availableRoomNumbers
                });
            }
        }

        return availabilityByType;
    }

}