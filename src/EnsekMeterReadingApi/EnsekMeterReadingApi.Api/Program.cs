using EnsekMeterReadingApi.Api.Controllers;
using EnsekMeterReadingApi.Api.Services;
using EnsekMeterReadingApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Register CsvMeterReader
builder.Services.AddScoped<ICsvMeterReading, CsvMeterReadingService>();

// Register MeterDbContext
builder.Services.AddDbContext<EnsekDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EnsekDbContext")));

var app = builder.Build();

// Configure the database and apply migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EnsekDbContext>();
    db.Database.EnsureDeleted();// In Production I recommend migrating manually instead, so EnsureDeleted is not used and Migrated would be wrapped in a DbContextFactory
    db.Database.Migrate();
}

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
