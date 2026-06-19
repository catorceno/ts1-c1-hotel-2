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
}