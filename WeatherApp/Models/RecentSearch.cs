using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace WeatherApp.Models
{
    [Table("recent_searches")]
    public class RecentSearch : BaseModel
    {
        [PrimaryKey("id", false)]
        public Guid Id { get; set; }

        [Column("city")]
        public string City { get; set; } = string.Empty;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("user_id")]
        public string UserId { get; set; } = string.Empty;
    }
}
