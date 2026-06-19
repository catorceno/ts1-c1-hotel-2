using Postgrest.Attributes;
using Postgrest.Models;

[Table("payment")]
public class Payment : BaseModel
{
    [PrimaryKey("id", false)]
    public long Id { get; set; }

    [Column("booking_id")]
    public long BookingId { get; set; }

    [Column("price_per_night")]
    public decimal PricePerNight { get; set; }

    [Column("total")]
    public decimal? Total { get; set; }
}
