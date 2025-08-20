using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain.Entities;

namespace MinimalApi.Infrastructure.DB;

public class ContextDb : DbContext
{
    private readonly IConfiguration? _configurationAppSettings;

    public ContextDb(IConfiguration configurationAppSettings)
    {
        _configurationAppSettings = configurationAppSettings;
    }

    public ContextDb(DbContextOptions<ContextDb> options) : base(options)
    {
    }

    public DbSet<Administrator> Administrators { get; set; } = default!;
    public DbSet<Vehicle> Vehicles { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Administrator>().HasData(
            new Administrator
            {
                Id = 1,
                Email = "asdf@asdf.com",
                Password = "123456",
                Profile = "admin"
            }
        );
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var stringConnection = _configurationAppSettings?.GetConnectionString("mysql")?.ToString();
            if (!string.IsNullOrEmpty(stringConnection))
                optionsBuilder.UseMySql(
                    stringConnection,
                    ServerVersion.AutoDetect(stringConnection)
                );
        }
    }
}