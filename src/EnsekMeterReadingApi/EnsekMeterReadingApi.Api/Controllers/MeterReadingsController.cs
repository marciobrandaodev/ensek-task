using CsvHelper;
using EnsekMeterReadingApi.Api.Services;
using EnsekMeterReadingApi.Core.Model;
using EnsekMeterReadingApi.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnsekMeterReadingApi.Api.Controllers;

[Route("")] // This route would match the acceptance criteria
[Route("api/v1/[controller]")]  //I would prefer to use a versioned route with controller name
public class MeterReadingsController(ICsvMeterReading csvMeterReading, ILogger<MeterReadingsController> logger, EnsekDbContext dbContext) : Controller
{
    private readonly ICsvMeterReading _csvMeterReading = csvMeterReading ?? throw new ArgumentNullException(nameof(csvMeterReading));
    private readonly ILogger<MeterReadingsController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly EnsekDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    [HttpPost("meter-reading-uploads")]
    public async Task<IActionResult> MeterReadingUploads(IFormFile file)
    {
        if (file == null || !file.FileName.EndsWith(".csv"))
        {
            return BadRequest("CSV file required!");
        }

        var results = new List<string>();
        await using var stream = file.OpenReadStream();
        var parsed = await _csvMeterReading.ReadMeterReadingsAsync(stream);

        foreach (var (dto, error) in parsed)
        {
            if (!string.IsNullOrEmpty(error))
            {
                results.Add(error);
                continue;
            }
            if (!await _dbContext.Accounts.AnyAsync(a => a.AccountId == dto.AccountId))
            {
                results.Add($"Unknown account {dto.AccountId}");
                continue;
            }

            var exists = await _dbContext.MeterReadings.AnyAsync(m =>
              m.AccountId == dto.AccountId &&
              m.MeterReadingDateTime == dto.MeterReadingDateTime);

            if (exists)
            {
                results.Add($"Duplicate: {dto.AccountId} @ {dto.MeterReadingDateTime}");
                continue;
            }
            _logger.LogInformation($"Adding reading for {dto.AccountId} @ {dto.MeterReadingDateTime} with value {dto.MeterReadValue}");
            _ = await _dbContext.MeterReadings.AddAsync(new MeterReading
            {
                AccountId = dto.AccountId,
                MeterReadingDateTime = dto.MeterReadingDateTime,
                MeterReadValue = dto.MeterReadValue
            });

        }
        _ = await _dbContext.SaveChangesAsync();
        return Ok(new { imported = _dbContext.ChangeTracker.Entries<MeterReading>().Count(), errors = results });
    }
}
