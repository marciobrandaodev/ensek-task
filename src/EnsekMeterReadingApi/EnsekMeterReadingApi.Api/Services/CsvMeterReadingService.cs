using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using EnsekMeterReadingApi.Api.DTO;
using System.Globalization;
using System.Text.RegularExpressions;

namespace EnsekMeterReadingApi.Api.Services;

public interface ICsvMeterReading
{
    Task<IEnumerable<(MeterReadingDto reading, string error)>> ReadMeterReadingsAsync(Stream stream);
}

public class CsvMeterReadingService : ICsvMeterReading
{
    public async Task<IEnumerable<(MeterReadingDto reading, string error)>> ReadMeterReadingsAsync(Stream stream)
    {
        using var reader = new StreamReader(stream);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var records = new List<(MeterReadingDto reading, string error)>();

        await foreach (var record in csv.GetRecordsAsync<MeterReadingDto>())
        {
            if(record == null)
            {
                records.Add((new MeterReadingDto(), "Invalid record – record is null"));
                continue;   // Skip null records
            }
            var line = csv?.Context?.Parser?.RawRecord;
            int rawVal = record.MeterReadValue;

            if (!Regex.IsMatch(rawVal.ToString(), @"^\d{1,5}$"))    //regex validation to allow any number from 0 to 99999
            {
                records.Add((new MeterReadingDto(), $"Invalid MeterReadValue '{rawVal}' – must be 1 to 5 digits"));
                continue;   // Skip invalid MeterReadValues
            }

            try
            {
                var dto = new MeterReadingDto
                {
                    AccountId = record.AccountId,
                    MeterReadingDateTime = record.MeterReadingDateTime,
                    MeterReadValue = record.MeterReadValue
                };
                records.Add((dto, string.Empty));
            }
            catch (Exception ex)
            {  // Capture any parsing errors and add to records
                records.Add((new MeterReadingDto(), $"Invalid row: {line?.Trim()} – {ex.Message}"));
            }
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
