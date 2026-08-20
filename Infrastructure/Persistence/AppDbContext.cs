using Microsoft.EntityFrameworkCore;
using PruebaTecnicaClt.Domain.Entities;

namespace PruebaTecnicaClt.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    public DbSet<Address> Addresses => Set<Address>();

    public DbSet<Currency> Currencies => Set<Currency>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(user => user.Id);

            entity.Property(user => user.Name)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(user => user.Email)
                .HasMaxLength(254)
                .UseCollation("NOCASE")
                .IsRequired();

            entity.HasIndex(user => user.Email)
                .IsUnique();

            entity.Property(user => user.IsActive)
                .HasDefaultValue(true)
                .IsRequired();

            entity.Property(user => user.PasswordHash)
                .HasMaxLength(500)
                .IsRequired();

            entity.HasMany(user => user.Addresses)
                .WithOne(address => address.User)
                .HasForeignKey(address => address.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(address => address.Id);

            entity.Property(address => address.Street)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(address => address.City)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(address => address.Country)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(address => address.ZipCode)
                .HasMaxLength(20);
        });

        modelBuilder.Entity<Currency>(entity =>
        {
            entity.HasKey(currency => currency.Id);

            entity.Property(currency => currency.Code)
                .HasMaxLength(3)
                .UseCollation("NOCASE")
                .IsRequired();

            entity.HasIndex(currency => currency.Code)
                .IsUnique();

            entity.Property(currency => currency.Name)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(currency => currency.RateToBase)
                .HasPrecision(18, 6)
                .IsRequired();

            entity.ToTable(tableBuilder => tableBuilder.HasCheckConstraint(
                "CK_Currencies_RateToBase_Positive",
                "RateToBase > 0"));
        });
    }
}
