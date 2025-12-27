using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace WeatherApp.Models;

[Table("favorite_cities")]
public class FavoriteCity : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("city")]
    public string City { get; set; } = string.Empty;

    [Column("user_id")]
    public string UserId { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}
