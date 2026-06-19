using Postgrest.Attributes;
using Postgrest.Models;

[Table("guest_booking")]
public class GuestBooking : BaseModel
{
    [PrimaryKey("id", false)]
    public long Id { get; set; }

    [Column("guest_id")]
    public long GuestId { get; set; }

    [Column("booking_id")]
    public long BookingId { get; set; }
}