using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
public enum RoomStatus
{
    [EnumMember(Value = "available")]
    Available,

    [EnumMember(Value = "reserved")]
    Reserved,

    [EnumMember(Value = "occupied")]
    Occupied,

    [EnumMember(Value = "maintenance")]
    Maintenance
}