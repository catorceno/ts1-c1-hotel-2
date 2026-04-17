public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepo;
    private readonly IRoomService _roomService;

    public PaymentService(IPaymentRepository paymentRepository, IRoomService roomService)
    {
        _paymentRepo = paymentRepository;
        _roomService = roomService;
    }

    public async Task<Payment> CreatePaymentForBooking(Booking booking)
    {
        var roomDetails = await _roomService.GetDetailsById(booking.RoomId);
        if (roomDetails == null)
            throw new Exception("No se encontró la habitación para el pago.");

        if (roomDetails.BasePrice <= 0)
            throw new Exception("El precio por noche de la habitación no es válido.");

        var payment = new Payment
        {
            BookingId = booking.Id,
            PricePerNight = roomDetails.BasePrice
        };

        return await _paymentRepo.CreateAsync(payment);
    }

    public async Task<Payment> UpdatePaymentTotalForBooking(Booking booking)
    {
        var payment = await _paymentRepo.GetByBookingIdAsync(booking.Id);
        if (payment == null)
            throw new Exception("No se encontró el pago asociado a la reserva.");
        
        var nights = booking.EndDate.DayNumber - booking.StartDate.DayNumber;
        if (nights <= 0)
            throw new Exception("El número de noches no es válido para calcular el total.");

        payment.Total = payment.PricePerNight * nights;
        return await _paymentRepo.UpdateAsync(payment);
    }

    public async Task<Payment> GetPaymentByBookingId(long bookingId)
    {
        return await _paymentRepo.GetByBookingIdAsync(bookingId);
    }
}