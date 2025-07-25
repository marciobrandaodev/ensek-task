using EnsekMeterReadingApi.Api.Services;
using System.ComponentModel;

namespace EnsekMeterReadingApi.Api.DTO;

public class MeterReadingDto
{
    public int AccountId { get; set; }
    [TypeConverter(typeof(MultiFormatDateTimeConverter))]
    public DateTime MeterReadingDateTime { get; set; }
    public int MeterReadValue { get; set; }
}
