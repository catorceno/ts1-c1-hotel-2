public class AvailableRoomsDto
{
    public string RoomType { get; set; }
    public List<AvailableRoomItemDto> AvailableRooms { get; set; }
}

public class AvailableRoomItemDto
{
    public long Id { get; set; }
    public string Number { get; set; }
}