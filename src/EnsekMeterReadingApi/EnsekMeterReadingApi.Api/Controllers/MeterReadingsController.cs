using CsvHelper;
using EnsekMeterReadingApi.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace EnsekMeterReadingApi.Api.Controllers;

[Route("")] // This route would match the acceptance criteria
[Route("api/v1/[controller]")]  //I would prefer to use a versioned route with controller name
public class MeterReadingsController(ICsvMeterReading csvMeterReading, ILogger<MeterReadingsController> logger) : Controller
{
    private readonly ICsvMeterReading _csvMeterReading = csvMeterReading ?? throw new ArgumentNullException(nameof(csvMeterReading));
    private readonly ILogger<MeterReadingsController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    [HttpPost("meter-reading-uploads")]
    public async Task<IActionResult> MeterReadingUploads(IFormFile file)
    {
        if (file == null || !file.FileName.EndsWith(".csv"))
        {
            return BadRequest("CSV file required!");
        }

        await using var stream = file.OpenReadStream();
        var records = await _csvMeterReading.ReadMeterReadingsAsync(stream);

        return Ok(new { imported = records.Count(), errors = "" });
    }
}
