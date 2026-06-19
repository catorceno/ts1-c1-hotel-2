using Postgrest.Attributes;
using Postgrest.Models;

[Table("room_type")]
public class RoomType : BaseModel
{
    [PrimaryKey("id", false)]
    public long Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("capacity")]
    public long Capacity { get; set; }

    [Column("base_price")]
    public decimal BasePrice { get; set; }
}
