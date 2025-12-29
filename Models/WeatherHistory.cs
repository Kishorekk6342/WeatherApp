using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace WeatherApp.Models;

[Table("weather_history")]
public class WeatherHistory : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("city")]
    public string City { get; set; } = string.Empty;

    [Column("weather_date")]
    public DateTime WeatherDate { get; set; }


    [Column("temp_min")]
    public decimal Temp_Min { get; set; }

    [Column("temp_max")]
    public decimal Temp_Max { get; set; }

    [Column("temp_avg")]
    public decimal Temp_Avg { get; set; }

    [Column("humidity")]
    public decimal Humidity { get; set; }

    [Column("wind_speed")]
    public decimal Wind_Speed { get; set; }

    [Column("condition")]
    public string Condition { get; set; } = string.Empty;
}
