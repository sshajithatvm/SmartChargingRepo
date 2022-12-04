using Microsoft.EntityFrameworkCore;

/// <summary>
/// The database context is the main class that coordinates Entity Framework functionality for a data model
/// </summary>
public class SmartChargingDbContext : DbContext
{
    public SmartChargingDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Group> Groups => Set<Group>();
    public DbSet<ChargeStation> ChargeStations => Set<ChargeStation>();
    public DbSet<Connector> Connectors => Set<Connector>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        base.OnConfiguring(options);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Connector>()
            .HasKey(x => new { x.ChargeStationId, x.ConnectorId });        

        modelBuilder.Entity<ChargeStation>()
                    .HasOne(e => e.Group)
                    .WithMany(e => e.ChargeStations)
                    .HasForeignKey(e => e.GroupId)
                    .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Connector>()
                    .HasOne(e => e.ChargeStation)
                    .WithMany(e => e.Connectors)
                    .HasForeignKey(e => e.ChargeStationId)               
                    .OnDelete(DeleteBehavior.Cascade);
   
    }
}
