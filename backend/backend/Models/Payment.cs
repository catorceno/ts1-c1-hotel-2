using Postgrest.Attributes;
using Postgrest.Models;

[Table("payment")]
public class Payment : BaseModel
{
    [PrimaryKey("id", false)]
    public long Id { get; set; }

    [Column("booking_id")]
    public long BookingId { get; set; }

    /// <summary>
    /// Precio por noche al momento de crear la reserva. Debe ser mayor a 0.
    /// </summary>
    [Column("price_per_night")]
    public decimal PricePerNight { get; set; }

    /// <summary>
    /// Nullable hasta el check-out. Se calcula como price_per_night * cantidad de noches.
    /// Debe ser mayor a 0 y mayor o igual a price_per_night cuando se asigna.
    /// Solo puede ser actualizado si booking.status == finalized.
    /// </summary>
    [Column("total")]
    public decimal? Total { get; set; }
}
