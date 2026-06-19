using System.Data.Common;
using AutoMapper;
using Moq;

namespace backend.Tests;

[TestFixture]
public class RoomServiceTests
{
    private Mock<IRoomRepository> _roomRepoMock;
    private Mock<IRoomTypeRepository> _roomTypeRepoMock;
    private Mock<IBookingRepository> _bookingRepoMock;

    private Mock<IMapper> _mapperMock;
    private RoomService _roomService;

    [SetUp]
    public void Setup()
    {
        _roomRepoMock = new Mock<IRoomRepository>();
        _roomTypeRepoMock = new Mock<IRoomTypeRepository>();
        _bookingRepoMock = new Mock<IBookingRepository>();

        _mapperMock = new Mock<IMapper>();
        _roomService = new RoomService(
            _roomRepoMock.Object,
            _bookingRepoMock.Object,
            _roomTypeRepoMock.Object,
            _mapperMock.Object
        );
    }

    [Test]
    public async Task IsAvailable_HabitacionDisponible_Exitoso()
    {
        // Arrange
        var roomId = 100;
        var startDate = new DateOnly(2026, 6, 1);
        var endDate = new DateOnly(2026, 6, 5);
        _bookingRepoMock.Setup(b => b.GetByRoomIdAsync(roomId)).ReturnsAsync(new List<Booking>());

        // Act
        var result = await _roomService.IsAvailable(roomId, startDate, endDate);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task IsAvailable_HabitacionDesocupadaMismoDia_Existoso()
    {
        // Arrange
        var roomId = 100;
        var startDate = new DateOnly(2026, 6, 1);
        var endDate = new DateOnly(2026, 6, 5);
        var bookingOverlap = new List<Booking>
        {
            new Booking
            {
                Id = 1,
                RoomId = roomId,
                StartDate = endDate,
                EndDate = new DateOnly(2026, 6, 6)
            }
        };
        _bookingRepoMock.Setup(b => b.GetByRoomIdAsync(roomId)).ReturnsAsync(bookingOverlap);

        // Act
        var result = await _roomService.IsAvailable(roomId, startDate, endDate);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task IsAvailable_HabitacionOcupada_Fallido()
    {
        // Arrange
        var roomId = 100;
        var startDate = new DateOnly(2026, 6, 1);
        var endDate = new DateOnly(2026, 6, 5);
        var bookingOverlap = new List<Booking>
        {
            new Booking
            {
                Id = 1,
                RoomId = roomId,
                StartDate = new DateOnly(2026, 6, 3),
                EndDate = new DateOnly(2026, 6, 6)
            }
        };
        _bookingRepoMock.Setup(b => b.GetByRoomIdAsync(roomId)).ReturnsAsync(bookingOverlap);

        // Act
        var result = await _roomService.IsAvailable(roomId, startDate, endDate);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task IsAvailable_FechaInicioPosteriorFechaFin_Fallido()
    {
        // Arrange
        var roomId = 100;
        var startDate = new DateOnly(2026, 6, 5);
        var endDate = new DateOnly(2026, 6, 1);

        // Act
        AsyncTestDelegate act = () => _roomService.IsAvailable(roomId, startDate, endDate);

        // Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(act);
        Assert.That(exception.Message, Is.EqualTo("La fecha de inicio debe ser anterior a la fecha de fin."));
    }

    [Test]
    public async Task IsAvailable_FechaInicioIgualFechaFin_Fallido()
    {
        // Arrange
        var roomId = 100;
        var startDate = new DateOnly(2026, 6, 1);
        var endDate = new DateOnly(2026, 6, 1);

        // Act
        AsyncTestDelegate act = () => _roomService.IsAvailable(roomId, startDate, endDate);

        // Assert
        var exception = Assert.ThrowsAsync<ArgumentException>(act);
        Assert.That(exception.Message, Is.EqualTo("La fecha de inicio debe ser anterior a la fecha de fin."));
    }

    [Test]
    public async Task GetAvailableRoomsByType()
    {
        // Arrange
        var startDate = new DateOnly(2026, 6, 1);
        var endDate = new DateOnly(2026, 6, 5);
        var roomIdOcupado = 1;
        var roomIdLibre = 50;
        var roomTypes = new List<RoomType>
        {
            new RoomType { Id = 1, Name = "Simple", Capacity = 1, BasePrice = 250m},
            new RoomType { Id = 2, Name = "Suite", Capacity = 1, BasePrice = 800m}
        };
        var rooms = new List<Room>
        {
            new Room { Id = roomIdOcupado, HotelId = 1, TypeId = 1, Number = "100"},
            new Room { Id = roomIdLibre, HotelId = 1, TypeId = 2, Number = "149"}
        };
        var bookings = new List<Booking>
        {
            new Booking { Id = 1, RoomId = roomIdOcupado, StartDate = new DateOnly(2026, 6, 3), EndDate = new DateOnly(2026, 6, 6) }
        };

        _roomTypeRepoMock.Setup(rt => rt.GetAllAsync()).ReturnsAsync(roomTypes);
        _roomRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(rooms);
        _bookingRepoMock.Setup(b => b.GetAllAsync()).ReturnsAsync(bookings);

        // Act
        var result = await _roomService.GetAvailableRoomsByType(startDate, endDate);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result.FirstOrDefault(r => r.RoomType == "Suite").AvailableRooms[0].Id, Is.EqualTo(roomIdLibre));
    }
}