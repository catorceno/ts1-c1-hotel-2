public interface IPaymentRepository
{
    Task<Payment> CreateAsync(Payment payment);
    Task<Payment> UpdateAsync(Payment payment);
    Task<Payment> GetByBookingIdAsync(long bookingId);
}