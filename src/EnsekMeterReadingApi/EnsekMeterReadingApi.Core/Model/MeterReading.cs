namespace EnsekMeterReadingApi.Core.Model;

public class MeterReading
{
    public Guid MeterReadingId { get; set; } = Guid.NewGuid();
    public int AccountId { get; set; }
    public DateTime MeterReadingDateTime { get; set; }
    public int MeterReadValue { get; set; }
    public Account? Account { get; set; }
}
