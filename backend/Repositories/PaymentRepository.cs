public class PaymentRepository : IPaymentRepository
{
    private readonly Supabase.Client _client;
    public PaymentRepository(Supabase.Client client)
    {
        _client = client;
    }

    public async Task<Payment> CreateAsync(Payment payment)
    {
        var response = await _client.From<Payment>().Insert(payment);
        return response.Models.First();
    }

    public async Task<Payment> UpdateAsync(Payment payment)
    {
        var response = await _client.From<Payment>().Update(payment);
        return response.Models.FirstOrDefault();
    }

    public async Task<Payment> GetByBookingIdAsync(long bookingId)
    {
        var response = await _client.From<Payment>().Where(p => p.BookingId == bookingId).Single();
        return response;
    }
}