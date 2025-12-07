namespace WeatherApp.Models;

public class WeatherResponse
{
    public WeatherMain Main { get; set; } = new();
    public WeatherWind Wind { get; set; } = new();
    public string Name { get; set; } = string.Empty;
}

public class WeatherMain
{
    public double Temp { get; set; }
    public double Temp_min { get; set; }
    public double Temp_max { get; set; }
    public int Humidity { get; set; }
}

public class WeatherWind
{
    public double Speed { get; set; }
}
