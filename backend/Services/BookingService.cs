using AutoMapper;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepo;
    private readonly IGuestBookingRepository _guestBookingRepo;
    private readonly IGuestRepository _guestRepo;
    private readonly IRoomService _roomService;
    private readonly IGuestService _guestService;
    private readonly IPaymentService _paymentService;
    private readonly IMapper _mapper;   
    public BookingService(
        IBookingRepository bookingRepository,
        IGuestBookingRepository guestBookingRepository,
        IGuestRepository guestRepository,
        IRoomService roomService,
        IGuestService guestService,
        IPaymentService paymentService,
        IMapper mapper
    )
    {
        _bookingRepo = bookingRepository;
        _guestBookingRepo = guestBookingRepository;
        _guestRepo = guestRepository;
        _roomService = roomService;
        _guestService = guestService;
        _paymentService = paymentService;
        _mapper = mapper;
    }

    public async Task<BookingResponseDto> CreateBooking(CreateBookingDto bookingDto)
    {
        if(bookingDto.EndDate <= bookingDto.StartDate)
            throw new Exception("Fecha inválida. La fecha de inicio debe ser antes de la fecha fin");
        
        var available = await _roomService.IsAvailable(bookingDto.RoomId, bookingDto.StartDate, bookingDto.EndDate);
        if(!available)
            throw new Exception("Habitación ocupada");
        
        // todo este bloque debería estar en una transacción, entonces si alguno falla nada se hace.
        // 1. crear guests
        var guestResponseDtos = new List<GuestResponseDto>();
        foreach (var guestDto in bookingDto.Guests)
        {
            var guestResponseDto = await _guestService.CreateOrGet(guestDto);
            guestResponseDtos.Add(guestResponseDto);
        }

        // 2. crear booking
        var booking = _mapper.Map<Booking>(bookingDto);
        booking.Status = BookingStatus.Reserved; // Establecer status por defecto
        var bookingCreated = await _bookingRepo.CreateAsync(booking);

        // 3. crear guest booking
        foreach(var guestReponseDto in guestResponseDtos)
        {
            var guestBooking = new GuestBooking
            {
                GuestId = guestReponseDto.Id,
                BookingId = bookingCreated.Id
            };
            await _guestBookingRepo.CreateAsync(guestBooking);
        }
        
        // 4. crear payment
        await _paymentService.CreatePaymentForBooking(bookingCreated);

        //

        var roomResponseDto = await _roomService.GetDetailsById(bookingCreated.RoomId);
        var bookingResponseDto = _mapper.Map<BookingResponseDto>(bookingCreated);
        bookingResponseDto.Room = roomResponseDto;
        bookingResponseDto.Guests = guestResponseDtos;

        return bookingResponseDto;
    }
    
    public async Task<BookingStatusResponseDto> CheckIn(long id)
    {
        var booking = await _bookingRepo.GetByIdAsync(id);

        if(booking.Status != BookingStatus.Reserved)
            throw new Exception("El huésped no puede hacer check-in.");
        
        var now = DateTimeOffset.UtcNow;
        var nowDate = DateOnly.FromDateTime(now.DateTime);
        if(nowDate < booking.StartDate || nowDate >= booking.EndDate)
            throw new Exception ("No se puede hacer check-in fuera del rango reservado.");

        booking.CheckIn = now;
        booking.Status = BookingStatus.Active;
        var bookingUpdated = await _bookingRepo.UpdateAsync(booking);

        return _mapper.Map<BookingStatusResponseDto>(bookingUpdated);
    }

    public async Task<BookingStatusResponseDto> CheckOut(long id)
    {
        var booking = await _bookingRepo.GetByIdAsync(id);

        if(booking.Status != BookingStatus.Active)
            throw new Exception("El huésped no puede hacer check-out.");
        
        var now = DateTimeOffset.UtcNow;
        var nowDate = DateOnly.FromDateTime(now.DateTime);
        if(nowDate < booking.StartDate || nowDate > booking.EndDate)
            throw new Exception ("No se puede hacer check-out fuera del rango reservado.");
        
        booking.CheckOut = now;
        booking.Status = BookingStatus.Finalized;

        // debería ser transacción
        // 1. actualizar booking
        var bookingUpdated = await _bookingRepo.UpdateAsync(booking);

        // 2. actualizar payment con el total calculado
        var paymentUpdated = await _paymentService.UpdatePaymentTotalForBooking(bookingUpdated);
        //

        var bookingResponseDto = _mapper.Map<BookingStatusResponseDto>(bookingUpdated);
        bookingResponseDto.Total = paymentUpdated.Total;

        return bookingResponseDto;
    }

    public async Task<List<BookingDetailsReponseDto>> GetBookings()
    {
        var bookings = await _bookingRepo.GetAllAsync();
        var bookingDetails = new List<BookingDetailsReponseDto>();

        foreach (var booking in bookings)
        {
            var roomResponseDto = await _roomService.GetDetailsById(booking.RoomId);
            var payment = await _paymentService.GetPaymentByBookingId(booking.Id);
            var guestResponseDtos = await _guestService.GetByBookingId(booking.Id);

            var bookingDetailsDto = new BookingDetailsReponseDto
            {
                Id = booking.Id,
                StartDate = booking.StartDate,
                EndDate = booking.EndDate,
                CheckIn = booking.CheckIn,
                CheckOut = booking.CheckOut,
                Status = booking.Status,
                Room = roomResponseDto,
                Payment = new PaymentResponseDto
                {
                    Id = payment.Id,
                    PricePerNight = payment.PricePerNight,
                    Total = payment.Total
                },
                Guests = guestResponseDtos
            };

            bookingDetails.Add(bookingDetailsDto);
        }

        return bookingDetails;
    }
}