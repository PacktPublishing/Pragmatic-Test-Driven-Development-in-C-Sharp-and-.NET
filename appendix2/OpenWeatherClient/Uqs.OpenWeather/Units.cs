using System.ComponentModel.DataAnnotations;

namespace Uqs.OpenWeather;

public enum Units
{
    [Display(Name = "standard")]
    Standard,
    [Display(Name = "metric")]
    Metric,
    [Display(Name = "imperial")]
    Imperial
}

