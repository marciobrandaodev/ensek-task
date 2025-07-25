using AutoFixture;
using AutoFixture.AutoMoq;
using EnsekMeterReadingApi.Api.DTO;
using EnsekMeterReadingApi.Api.Services;
using Moq;

namespace EnsekMeterReadingApi.Tests.EnsekMeterReadingApi.Api.Services;

[TestFixture]
public class CsvMeterReadingServiceTests
{
    private IFixture _fixture;
    private Mock<ICsvMeterReading> _csvMeterReadingMock;
    private Stream _stream;

    [SetUp]
    public void SetUp()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _csvMeterReadingMock = _fixture.Freeze<Mock<ICsvMeterReading>>();
        _stream = new MemoryStream();
    }

    [TestCase(5)]
    [TestCase(1)]
    [TestCase(0)]
    public async Task ReadMeterReadingsAsync_ShouldReturnMeterReadingDtos_WhenStreamIsValid(int numberOfReads)
    {
        // Arrange
        var expectedRecords = _fixture.CreateMany<(MeterReadingDto reading, string error)>(numberOfReads).ToList();
        _csvMeterReadingMock.Setup(x => x.ReadMeterReadingsAsync(It.IsAny<Stream>()))
            .ReturnsAsync(expectedRecords);
        // Act
        var result = await _csvMeterReadingMock.Object.ReadMeterReadingsAsync(_stream);
        // Assertions;
        // I would normally write 3 different tests with a single assertion each, but for brevity, I will combine them here.
        Assert.That(result, Is.Not.Null);
        Assert.That(expectedRecords.Count, Is.EqualTo(result.Count()));
        Assert.That(expectedRecords, Is.EqualTo(result.ToList()));
    }

    [Test]
    public async Task ReadMeterReadingsAsync_ShouldThrowException_WhenStreamIsInvalid()
    {
        // Arrange
        _csvMeterReadingMock.Setup(x => x.ReadMeterReadingsAsync(It.IsAny<Stream>()))
            .ThrowsAsync(new InvalidOperationException("Invalid stream"));
        
        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(async () => 
            await _csvMeterReadingMock.Object.ReadMeterReadingsAsync(_stream));
    }

}
