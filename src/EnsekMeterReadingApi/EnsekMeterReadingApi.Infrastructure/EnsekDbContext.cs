using EnsekMeterReadingApi.Core.Model;
using Microsoft.EntityFrameworkCore;

namespace EnsekMeterReadingApi.Infrastructure;

public class EnsekDbContext : DbContext
{
    public EnsekDbContext() // Default constructor to be used by EF Core migrations
        : base()
    {
    }
    public EnsekDbContext(DbContextOptions<EnsekDbContext> options)
        : base(options)
    {
    }

    public DbSet<Account> Accounts { get; set; } = null!;
    public DbSet<MeterReading> MeterReadings { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Data Source=localhost,1433;Database=EnsekDb;Encrypt=True;TrustServerCertificate=True;");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Account>()
            .HasKey(a => a.AccountId);

        modelBuilder.Entity<Account>()
            .HasMany(a => a.MeterReadings)
            .WithOne(m => m.Account)
            .HasForeignKey(m => m.AccountId);

        modelBuilder.Entity<MeterReading>()
            .HasKey(m => m.MeterReadingId);

        modelBuilder.Entity<MeterReading>()
            .HasOne(m => m.Account)
            .WithMany(a => a.MeterReadings)
            .HasForeignKey(m => m.AccountId);


        // Seed Data from provided Test_Accounts.csv
        var accounts = File
          .ReadAllLines("./data/Test_Accounts.csv")
          .Skip(1)  //Skip header line
          .Select(line => line.Split(','))
          .Select(cols => new Account
          {
              AccountId = int.Parse(cols[0]),
              FirstName = cols[1],
              LastName = cols[2]
          });

        modelBuilder.Entity<Account>().HasData(accounts);
    }
}