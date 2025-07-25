using EnsekMeterReadingApi.Api.Services;
using CsvHelper.Configuration.Attributes;

namespace EnsekMeterReadingApi.Api.DTO;

public class MeterReadingDto
{
    public int AccountId { get; set; }
    [TypeConverter(typeof(MultiFormatDateTimeConverter))]
    public DateTime MeterReadingDateTime { get; set; }
    public int MeterReadValue { get; set; }
}
