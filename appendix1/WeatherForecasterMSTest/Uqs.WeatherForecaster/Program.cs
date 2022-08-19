using Uqs.WeatherForecaster.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IOpenWeatherService>(_ => {
    string apiKey = builder.Configuration["OpenWeather:Key"];
    HttpClient httpClient = new HttpClient();
    return new OpenWeatherService(apiKey, httpClient);
});
builder.Services.AddSingleton<IWeatherAnalysisService, WeatherAnalysisService>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
