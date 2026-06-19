public class RoomResponseDto
{
    public long Id { get; set; }
    public required string Number { get; set; }
    public required string Type { get; set; }
    public long Capacity { get; set; }
    public decimal BasePrice { get; set; }
}