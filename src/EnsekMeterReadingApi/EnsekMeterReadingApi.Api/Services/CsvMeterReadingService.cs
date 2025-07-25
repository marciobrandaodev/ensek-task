using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using EnsekMeterReadingApi.Api.DTO;
using System.Globalization;

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
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        // Read the CSV file and map it to MeterReadingDto
        var records = new List<MeterReadingDto>();
        await foreach (var record in csv.GetRecordsAsync<MeterReadingDto>())
        {
            records.Add(record);
        }

        return records;
    }
}

public class MultiFormatDateTimeConverter : DefaultTypeConverter
{
    private static readonly string[] formats = new[]
    {
        "dd/MM/yyyy HH:mm",
        "MM/dd/yyyy HH:mm",
        "yyyy-MM-dd HH:mm",
        "dd/MM/yyyy",
        "MM/dd/yyyy",
        "yyyy-MM-dd",
        "yyyy/MM/dd HH:mm:ss",
        "yyyy/MM/dd"
    };

    public override object ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new TypeConverterException(this, memberMapData, text, row.Context, "DateTime value cannot be null or empty.");
        }

        if (DateTime.TryParseExact(text, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
        {
            return dt;
        }
        if (DateTime.TryParse(text, out dt))
        {
            return dt;
        }

        throw new TypeConverterException(this, memberMapData, text, row.Context, $"Could not parse '{text}' as DateTime.");
    }
}
