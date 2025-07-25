using EnsekMeterReadingApi.Core.Model;
using EnsekMeterReadingApi.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Moq;
using MockQueryable.Moq;

namespace EnsekMeterReadingApi.Tests.EnsekMeterReadingApi.Infrastructure;

[TestFixture]
public class EnsekDbContextTests
{
    private Mock<EnsekDbContext> _dbContextMock;
    [SetUp]
    public void SetUp()
    {
        var meterReadingData = new List<MeterReading>
        {
            new MeterReading { AccountId = 1, MeterReadingDateTime = DateTime.Now, MeterReadValue = 100 },
            new MeterReading { AccountId = 2, MeterReadingDateTime = DateTime.Now.AddDays(-1), MeterReadValue = 200 }
        };

        var accountData = new List<Account>
        {
            new Account { AccountId = 1, FirstName = "John", LastName = "Doe" },
            new Account { AccountId = 2, FirstName = "Jane", LastName = "Smith" }
        };

        var meterReadingsMock = meterReadingData.AsQueryable().BuildMockDbSet();
        var accountsMock = accountData.AsQueryable().BuildMockDbSet();

        _dbContextMock = new Mock<EnsekDbContext>();
        _dbContextMock.Setup(db => db.MeterReadings).Returns(meterReadingsMock.Object);
        _dbContextMock.Setup(db => db.Accounts).Returns(accountsMock.Object);
    }


    [Test]
    public void EnsekDbContext_ShouldInitializeWithDefaultConstructor()
    {
        // Arrange & Act
        var context = new EnsekDbContext();
        // Assert
        Assert.That(context, Is.Not.Null);
        Assert.That(context, Is.InstanceOf<EnsekDbContext>());
    }

    [Test]
    public void EnsekDbContext_ShouldInitializeWithOptionsConstructor()
    {
        // Arrange
        var options = new DbContextOptions<EnsekDbContext>();
        // Act
        var context = new EnsekDbContext(options);
        // Assert
        Assert.That(context, Is.Not.Null);
        Assert.That(context, Is.InstanceOf<EnsekDbContext>());
    }

    [Test]
    public void EnsekDbContext_ShouldHaveAccountsAndMeterReadingsDbSets()
    {
        //Arrange
        var dbContext = _dbContextMock.Object;
        // Act
        var accountsDbSet = dbContext.Accounts;
        var meterReadingsDbSet = dbContext.MeterReadings;
        // Assert
        Assert.That(accountsDbSet, Is.Not.Null);
        Assert.That(meterReadingsDbSet, Is.Not.Null);
    }

    [Test]
    public void EnsekDbContext_ShouldHaveMeterReadingWithAccountForeignKey()
    {
        //Arrange
        var dbContext = _dbContextMock.Object;
        // Act
        var meterReading = dbContext.MeterReadings.FirstOrDefault();
        var account = dbContext.Accounts.Single(a => a.AccountId == meterReading.AccountId);
        // Assert
        Assert.That(meterReading, Is.Not.Null);
        Assert.That(meterReading.AccountId, Is.EqualTo(account.AccountId));
    }
}
