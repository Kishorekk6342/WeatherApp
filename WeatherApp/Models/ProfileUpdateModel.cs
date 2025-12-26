using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace WeatherApp.Models
{
    [Table("profiles")]
    public class ProfileUpdateModel : BaseModel
    {
        [PrimaryKey("id", false)]
        public Guid Id { get; set; }

        [Column("username")]
        public string? Username { get; set; }

        [Column("avatar_url")]
        public string? AvatarUrl { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
