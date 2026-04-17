using AutoMapper;

public class GuestService : IGuestService
{
    private readonly IGuestRepository _guestRepo;
    private readonly IGuestBookingRepository _guestBookingRepo;
    private readonly IMapper _mapper;   
    public GuestService(IGuestRepository guestRepository, IGuestBookingRepository guestBookingRepository, IMapper mapper)
    {
        _guestRepo = guestRepository;
        _guestBookingRepo = guestBookingRepository;
        _mapper = mapper;
    }

    public async Task<GuestResponseDto?> FindByCi(string ci)
    {
        var response = await _guestRepo.GetAllAsync();
        var guest = response.FirstOrDefault(g => g.Ci == ci);
        return guest != null ? _mapper.Map<GuestResponseDto>(guest) : null;
    }
    public async Task<GuestResponseDto> CreateOrGet(CreateGuestDto guestDto)
    {
        var existingGuestDto = await FindByCi(guestDto.Ci);
        if (existingGuestDto != null)
        {
            return existingGuestDto;
        }
        
        var guest = _mapper.Map<Guest>(guestDto);
        var guestCreated = await _guestRepo.CreateAsync(guest);
        return _mapper.Map<GuestResponseDto>(guestCreated);
    }
    
    public async Task<List<GuestResponseDto>> GetByBookingId(long bookingId)
    {
        var guestBookings = await _guestBookingRepo.GetByBookingIdAsync(bookingId);
        var guests = new List<GuestResponseDto>();

        foreach (var guestBooking in guestBookings)
        {
            var guest = await _guestRepo.GetByIdAsync(guestBooking.GuestId);
            var guestDto = _mapper.Map<GuestResponseDto>(guest);
            guests.Add(guestDto);
        }

        return guests;
    }
}