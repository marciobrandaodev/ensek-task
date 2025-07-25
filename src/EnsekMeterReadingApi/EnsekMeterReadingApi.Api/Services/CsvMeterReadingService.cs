using EnsekMeterReadingApi.Api.DTO;

namespace EnsekMeterReadingApi.Api.Services;

public interface ICsvMeterReading
{
    Task<IEnumerable<MeterReadingDto>> ReadMeterReadingsAsync(Stream stream);
}

public class CsvMeterReadingService : ICsvMeterReading
{
    public async Task<IEnumerable<MeterReadingDto>> ReadMeterReadingsAsync(Stream stream)
    {
        using var reader = new StreamReader(stream);
        using var csv = new CsvHelper.CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture);

        // Read the CSV file and map it to MeterReadingDto
        var records = new List<MeterReadingDto>();
        await foreach (var record in csv.GetRecordsAsync<MeterReadingDto>())
        {
            records.Add(record);
        }

        return records;
    }
}
