public interface IGuestService
{
    Task<GuestResponseDto> FindByCi(string ci);
    Task<GuestResponseDto> CreateOrGet(CreateGuestDto guestDto);
    Task<List<GuestResponseDto>> GetByBookingId(long bookingId);
}