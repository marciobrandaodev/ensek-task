using AutoFixture;
using AutoFixture.AutoMoq;
using EnsekMeterReadingApi.Api.DTO;
using EnsekMeterReadingApi.Api.Services;
using Moq;
using NUnit.Framework;

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

    [Test]
    public async Task ReadMeterReadingsAsync_ShouldReturnMeterReadingDtos_WhenStreamIsValid()
    {
        // Arrange
        var expectedRecords = _fixture.CreateMany<MeterReadingDto>(5).ToList();
        _csvMeterReadingMock.Setup(x => x.ReadMeterReadingsAsync(It.IsAny<Stream>()))
            .ReturnsAsync(expectedRecords);
        // Act
        var result = await _csvMeterReadingMock.Object.ReadMeterReadingsAsync(_stream);
        // Assert
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
