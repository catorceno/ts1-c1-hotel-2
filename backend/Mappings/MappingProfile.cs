using AutoMapper;

namespace backend.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Guest mappings
        CreateMap<Guest, GuestResponseDto>().ReverseMap();
        CreateMap<CreateGuestDto, Guest>();

        // Booking mappings
        CreateMap<Booking, BookingResponseDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status ?? BookingStatus.Reserved));
        CreateMap<CreateBookingDto, Booking>();
        CreateMap<Booking, BookingStatusResponseDto>();

        // Room mappings
        CreateMap<Room, RoomResponseDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.RoomType != null ? src.RoomType.Name : string.Empty))
            .ForMember(dest => dest.Capacity, opt => opt.MapFrom(src => src.RoomType != null ? src.RoomType.Capacity : 0))
            .ForMember(dest => dest.BasePrice, opt => opt.MapFrom(src => src.RoomType != null ? src.RoomType.BasePrice : 0));

        // RoomType mappings
        CreateMap<RoomType, Room>().ReverseMap();
    }
}
