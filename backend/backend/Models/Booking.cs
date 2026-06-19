using Newtonsoft.Json;
using Postgrest.Attributes;
using Postgrest.Models;

[Table("booking")]
public class Booking : BaseModel
{
    [PrimaryKey("id", false)]
    public long Id { get; set; }

    [Column("room_id")]
    public long RoomId { get; set; }

    [Column("start_date")]
    public DateOnly StartDate { get; set; }

    [Column("end_date")]
    public DateOnly EndDate { get; set; }

    [Column("check-in")]
    public DateTimeOffset? CheckIn { get; set; }

    [Column("check-out")]
    public DateTimeOffset? CheckOut { get; set; }

    [Column("status", NullValueHandling = NullValueHandling.Ignore)]
    public BookingStatus? Status { get; set; }
}