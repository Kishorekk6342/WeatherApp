using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace WeatherApp.Models;

[Table("recent_searches")]
public class RecentSearch : BaseModel
{
    [PrimaryKey("id")]
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("city")]
    public string City { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}
