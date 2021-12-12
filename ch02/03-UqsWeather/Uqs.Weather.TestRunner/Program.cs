using Microsoft.Extensions.Logging.Abstractions;
using Uqs.Weather.Controllers;
var logger = NullLogger<WeatherForecastController>.Instance;
var controller = new WeatherForecastController(logger, null!, null!, null!);
double f1 = controller.ConvertCToF(-1.0);
if (f1 != 30.20d) throw new Exception("Invalid conversion");
double f2 = controller.ConvertCToF(1.2);
if (f2 != 34.16d) throw new Exception("Invalid conversion");
Console.WriteLine("Test Passed");