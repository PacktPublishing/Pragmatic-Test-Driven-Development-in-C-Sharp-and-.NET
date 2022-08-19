using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Uqs.OpenWeather;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
public record OneCallResponse
{
    [JsonProperty("lat")]
    public double Lat { get; set; }

    [JsonProperty("lon")]
    public double Lon { get; set; }

    [JsonProperty("timezone")]
    public string Timezone { get; set; }

    [JsonProperty("timezone_offset")]
    public long TimezoneOffset { get; set; }

    [JsonProperty("daily")]
    public Daily[] Daily { get; set; }
}

public record Daily
{
    [JsonProperty("dt"), JsonConverter(typeof(UnixDateTimeConverter))]
    public DateTime Dt { get; set; }

    [JsonProperty("sunrise")]
    public long Sunrise { get; set; }

    [JsonProperty("sunset")]
    public long Sunset { get; set; }

    [JsonProperty("moonrise")]
    public long Moonrise { get; set; }

    [JsonProperty("moonset")]
    public long Moonset { get; set; }

    [JsonProperty("moon_phase")]
    public double MoonPhase { get; set; }

    [JsonProperty("temp")]
    public Temp Temp { get; set; }

    [JsonProperty("feels_like")]
    public FeelsLike FeelsLike { get; set; }

    [JsonProperty("pressure")]
    public long Pressure { get; set; }

    [JsonProperty("humidity")]
    public long Humidity { get; set; }

    [JsonProperty("dew_point")]
    public double DewPoint { get; set; }

    [JsonProperty("wind_speed")]
    public double WindSpeed { get; set; }

    [JsonProperty("wind_deg")]
    public long WindDeg { get; set; }

    [JsonProperty("wind_gust")]
    public double WindGust { get; set; }

    [JsonProperty("weather")]
    public Weather[] Weather { get; set; }

    [JsonProperty("clouds")]
    public long Clouds { get; set; }

    [JsonProperty("pop")]
    public double Pop { get; set; }

    [JsonProperty("uvi")]
    public double Uvi { get; set; }

    [JsonProperty("rain")]
    public double? Rain { get; set; }

    [JsonProperty("snow")]
    public double? Snow { get; set; }
}

public record FeelsLike
{
    [JsonProperty("day")]
    public double Day { get; set; }

    [JsonProperty("night")]
    public double Night { get; set; }

    [JsonProperty("eve")]
    public double Eve { get; set; }

    [JsonProperty("morn")]
    public double Morn { get; set; }
}

public record Temp
{
    [JsonProperty("day")]
    public double Day { get; set; }

    [JsonProperty("min")]
    public double Min { get; set; }

    [JsonProperty("max")]
    public double Max { get; set; }

    [JsonProperty("night")]
    public double Night { get; set; }

    [JsonProperty("eve")]
    public double Eve { get; set; }

    [JsonProperty("morn")]
    public double Morn { get; set; }
}

public record Weather
{
    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("main")]
    public string Main { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("icon")]
    public string Icon { get; set; }
}

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

