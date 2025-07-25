namespace EnsekMeterReadingApi.Core.Model;

public class Account
{
    public int AccountId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public ICollection<MeterReading> MeterReadings { get; set; } = [];
}
