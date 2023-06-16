using Microsoft.EntityFrameworkCore;
using ShippingService.Entities;

namespace ShippingService.Data;

public class ApplicationContext : DbContext
{
    private readonly string _connectionString;

    public ApplicationContext(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Default")!;
        Database.EnsureCreated();
    }

    public DbSet<Cargo> Cargos => Set<Cargo>();
    public DbSet<CargoCompartment> CargoCompartments => Set<CargoCompartment>();
    public DbSet<MooredShip> MooredShips => Set<MooredShip>();
    public DbSet<Seaport> Seaports => Set<Seaport>();
    public DbSet<Ship> Ships => Set<Ship>();
    public DbSet<Shipping> Shippings => Set<Shipping>();
    public DbSet<Storage> Storages => Set<Storage>();
    public DbSet<TransportProtocol> TransportProtocols => Set<TransportProtocol>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql(_connectionString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Shipping>()
            .HasOne(shipping => shipping.FromSeaport)
            .WithMany(seaport => seaport.FromShippings);

        modelBuilder.Entity<Shipping>()
            .HasOne(shipping => shipping.ToSeaport)
            .WithMany(seaport => seaport.ToShippings);
    }
}