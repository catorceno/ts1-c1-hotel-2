using System.Data.Common;
using AutoMapper;
using Moq;

namespace backend.Tests;

[TestFixture]
public class BookingServiceTests
{
    private Mock<IBookingRepository> _bookingRepoMock;
    private Mock<IGuestBookingRepository> _guestBookingRepoMock;
    private Mock<IGuestRepository> _guestRepoMock;

    private Mock<IRoomService> _roomServiceMock;
    private Mock<IGuestService> _guestServiceMock;
    private Mock<IPaymentService> _paymentServiceMock;

    private Mock<IMapper> _mapperMock;
    private BookingService _bookingService;

    [SetUp]
    public void Setup()
    {
        _bookingRepoMock = new Mock<IBookingRepository>();
        _guestBookingRepoMock = new Mock<IGuestBookingRepository>();
        _guestRepoMock = new Mock<IGuestRepository>();

        _roomServiceMock = new Mock<IRoomService>();
        _guestServiceMock = new Mock<IGuestService>();
        _paymentServiceMock = new Mock<IPaymentService>();

        _mapperMock = new Mock<IMapper>();
        _bookingService = new BookingService(
            _bookingRepoMock.Object,
            _guestBookingRepoMock.Object,
            _guestRepoMock.Object,

            _roomServiceMock.Object,
            _guestServiceMock.Object,
            _paymentServiceMock.Object,
                
            _mapperMock.Object
        );
    }

    [Test]
    public async Task CreateBooking_FechaValidaYHabitacionDisponible_Exitoso()
    {
        // Arrange
        var bookingId = 100;
        var guestId = 1;
        var name = "Juan Marquez";
        var ci = "8673020";
        var phone = "77999910";
        var roomId = 100;
        var basePrice = 250m;
        var startDate = new DateOnly(2026, 6, 1);
        var endDate = new DateOnly(2026, 6, 5);
        var guestDto = new CreateGuestDto
        {
            Name = name,
            Ci = ci,
            Phone = phone
        };
        var dto = new CreateBookingDto
        {
            StartDate = startDate,
            EndDate = endDate,
            RoomId = roomId,
            Guests = new List<CreateGuestDto>{ guestDto }
        };

        var guestResponse = new GuestResponseDto
        {
            Id = guestId,
            Name = name,
            Ci = ci,
            Phone = phone
        };
        var bookingCreated = new Booking
        {
            Id = bookingId,
            RoomId = roomId,
            StartDate = startDate,
            EndDate = endDate,
            Status = BookingStatus.Reserved
        };
        var guestBooking = new GuestBooking
        {
            GuestId = guestId,
            BookingId = bookingId
        };
        var guestBookingCreated = new GuestBooking
        {
            Id = 1,
            GuestId = guestId,
            BookingId = bookingId
        };
        var roomResponse = new RoomResponseDto
        {
            Id = roomId,
            BasePrice = basePrice,
            Type = "Simple"
        };
        var paymentCreated = new Payment
        {
            Id = 1,
            BookingId = bookingId,
            PricePerNight = basePrice
        };
        var bookingResponse = new BookingResponseDto
        {
            Id = bookingId,
            Status = BookingStatus.Reserved
        };

        _roomServiceMock.Setup(r => r.IsAvailable(roomId, startDate, endDate)).ReturnsAsync(true);
        _guestServiceMock.Setup(g => g.CreateOrGet(guestDto)).ReturnsAsync(guestResponse);

        _mapperMock.Setup(m => m.Map<Booking>(dto)).Returns(bookingCreated);

        _bookingRepoMock.Setup(b => b.CreateAsync(bookingCreated)).ReturnsAsync(bookingCreated);
        _guestBookingRepoMock.Setup(gb => gb.CreateAsync(guestBooking)).ReturnsAsync(guestBookingCreated);

        _paymentServiceMock.Setup(p => p.CreatePaymentForBooking(bookingCreated)).ReturnsAsync(paymentCreated);
        _roomServiceMock.Setup(r => r.GetDetailsById(roomId)).ReturnsAsync(roomResponse);

        _mapperMock.Setup(m => m.Map<BookingResponseDto>(bookingCreated)).Returns(bookingResponse);

        // Act
        var result = await _bookingService.CreateBooking(dto);

        // Assert
        Assert.That(result.Status, Is.EqualTo(BookingStatus.Reserved));
        _paymentServiceMock.Verify(p => p.CreatePaymentForBooking(It.Is<Booking>(b => b.Status == BookingStatus.Reserved)), Times.Once);
        _guestBookingRepoMock.Verify(gb => gb.CreateAsync(It.Is<GuestBooking>(gb => gb.GuestId == guestResponse.Id)), Times.Once);
    }

    [Test]
    public async Task CreateBooking_FechaInicioPosteriorFechaFin_Fallido()
    {
        // Arrange
        var name = "Juan Marquez";
        var ci = "8673020";
        var phone = "77999910";
        var roomId = 100;
        var startDate = new DateOnly(2026, 6, 5);
        var endDate = new DateOnly(2026, 6, 1);
        var guestDto = new CreateGuestDto
        {
            Name = name,
            Ci = ci,
            Phone = phone
        };
        var dto = new CreateBookingDto
        {
            StartDate = startDate,
            EndDate = endDate,
            RoomId = roomId,
            Guests = new List<CreateGuestDto>{ guestDto }
        };

        // Act
        AsyncTestDelegate act = () => _bookingService.CreateBooking(dto);

        // Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(act);
        Assert.That(exception.Message, Is.EqualTo("Fecha inválida. La fecha de inicio debe ser antes de la fecha fin"));
    }

    [Test]
    public async Task CreateBooking_HabitacionOcupada_Fallido()
    {
        // Arrange
        var name = "Juan Marquez";
        var ci = "8673020";
        var phone = "77999910";
        var roomId = 100;
        var startDate = new DateOnly(2026, 6, 1);
        var endDate = new DateOnly(2026, 6, 5);
        var guestDto = new CreateGuestDto
        {
            Name = name,
            Ci = ci,
            Phone = phone
        };
        var dto = new CreateBookingDto
        {
            StartDate = startDate,
            EndDate = endDate,
            RoomId = roomId,
            Guests = new List<CreateGuestDto>{ guestDto }
        };
        _roomServiceMock.Setup(r => r.IsAvailable(roomId, startDate, endDate)).ReturnsAsync(false);

        // Act
        AsyncTestDelegate act = () => _bookingService.CreateBooking(dto);

        // Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(act);
        Assert.That(exception.Message, Is.EqualTo("Habitación ocupada"));
    }

    [Test]
    public async Task GetBookings_ExistenRegistros_Exitoso()
    {
        // Arrange
        var bookingId = 100;
        var guestId = 1;
        var name = "Juan Marquez";
        var ci = "8673020";
        var phone = "77999910";
        var roomId = 1;
        var basePrice = 250m;
        var startDate = new DateOnly(2026, 6, 1);
        var endDate = new DateOnly(2026, 6, 5);

        var bookings = new List<Booking>
        {
            new Booking
            {
                Id = bookingId,
                RoomId = roomId,
                StartDate = startDate,
                EndDate = endDate,
                Status = BookingStatus.Reserved
            }
        };
        var guestResponseList = new List<GuestResponseDto>
        {
            new GuestResponseDto
            {
                Id = guestId,
                Name = name,
                Ci = ci,
                Phone = phone
            }
        };
        var roomResponse = new RoomResponseDto
        {
            Id = roomId,
            BasePrice = basePrice,
            Type = "Simple"
        };
        var payment = new Payment
        {
            Id = 1,
            BookingId = bookingId,
            PricePerNight = basePrice
        };

        _bookingRepoMock.Setup(b => b.GetAllAsync()).ReturnsAsync(bookings);
        _roomServiceMock.Setup(r => r.GetDetailsById(roomId)).ReturnsAsync(roomResponse);
        _paymentServiceMock.Setup(p => p.GetPaymentByBookingId(bookingId)).ReturnsAsync(payment);
        _guestServiceMock.Setup(g => g.GetByBookingId(bookingId)).ReturnsAsync(guestResponseList);

        // Act
        var result = await _bookingService.GetBookings();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(1));
    }
}