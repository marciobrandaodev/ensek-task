using EnsekMeterReadingApi.Api.Controllers;
using EnsekMeterReadingApi.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Register CsvMeterReader
builder.Services.AddScoped<ICsvMeterReading, CsvMeterReadingService>();

var app = builder.Build();

// Register the logger
_ = app.Services.GetRequiredService<ILogger<MeterReadingsController>>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
