using Postgrest.Attributes;
using Postgrest.Models;

[Table("room")]
public class Room : BaseModel
{
    [PrimaryKey("id", false)]
    public long Id { get; set; }

    [Column("hotel_id")]
    public long HotelId { get; set; }

    [Column("type_id")]
    public long TypeId { get; set; }

    [Column("number")]
    public string Number { get; set; } = string.Empty;

    [Column("current_status")]
    public RoomStatus CurrentStatus { get; set; }

    public RoomType? RoomType { get; set; }
}