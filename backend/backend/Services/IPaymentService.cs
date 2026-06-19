public interface IPaymentService
{
    Task<Payment> CreatePaymentForBooking(Booking booking);
    Task<Payment> UpdatePaymentTotalForBooking(Booking booking);
    Task<Payment> GetPaymentByBookingId(long bookingId);
}