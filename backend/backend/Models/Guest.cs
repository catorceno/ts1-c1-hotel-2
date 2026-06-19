using Postgrest.Attributes;
using Postgrest.Models;

[Table("guest")]
public class Guest : BaseModel
{
    [PrimaryKey("id", false)]
    public long Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("ci")]
    public string Ci { get; set; } = string.Empty;

    [Column("phone")]
    public string? Phone { get; set; }
}