using AutoFixture;
using EnsekMeterReadingApi.Api.Controllers;
using EnsekMeterReadingApi.Api.Services;
using Moq;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using EnsekMeterReadingApi.Api.DTO;
using Microsoft.AspNetCore.Mvc;

namespace EnsekMeterReadingApi.Tests.EnsekMeterReadingApi.Api.Controllers;

[TestFixture]
public class MeterReadingsControllerTests
{
    private IFixture _fixture;
    private Mock<ICsvMeterReading> _csvMeterReadingMock;
    private MeterReadingsController _controller;

    [SetUp]
    public void SetUp()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _csvMeterReadingMock = _fixture.Freeze<Mock<ICsvMeterReading>>();
        var logger = _fixture.Freeze<Mock<ILogger<MeterReadingsController>>>();
        _controller = new MeterReadingsController(_csvMeterReadingMock.Object, logger.Object);
    }

    [TestCase(5)]
    [TestCase(1)]
    [TestCase(0)]
    public async Task MeterReadingUploads_ShouldReturnOk_WhenFileIsValid(int numberOfReadings)
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.FileName).Returns("valid.csv");
        fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());
        
        var expectedRecords = _fixture.CreateMany<MeterReadingDto>(numberOfReadings).ToList();
        _csvMeterReadingMock.Setup(x => x.ReadMeterReadingsAsync(It.IsAny<Stream>()))
            .ReturnsAsync(expectedRecords);
        
        // Act
        var result = await _controller.MeterReadingUploads(fileMock.Object);
        
        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult.Value, Is.Not.Null);
    }

    [Test]
    public async Task MeterReadingUploads_ShouldReturnBadRequest_WhenFileIsNull()
    {
        // Arrange
        IFormFile file = null;
        // Act
        var result = await _controller.MeterReadingUploads(file);
        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult.Value, Is.EqualTo("CSV file required!"));
    }

}
