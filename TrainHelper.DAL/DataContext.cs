using Microsoft.EntityFrameworkCore;
using TrainHelper.DAL.Entities;

namespace TrainHelper.DAL;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Car>()
            .HasIndex(a => a.CarNumber)
            .IsUnique();

        modelBuilder.Entity<Invoice>()
            .HasIndex(a => a.InvoiceNum)
            .IsUnique();

        modelBuilder.Entity<Station>()
            .HasIndex(a => a.StationName)
            .IsUnique();

        modelBuilder.Entity<Train>()
            .HasIndex(a => a.Number)
            .IsUnique();

        modelBuilder.Entity<CarOperation>()
            .HasIndex(c => c.OperationName)
            .IsUnique();

        modelBuilder.Entity<Freight>()
            .HasIndex(f => f.FreightEtsngName)
            .IsUnique();

        modelBuilder.Entity<WayPoint>()
            .HasIndex("CarId", nameof(WayPoint.OperationDate), "StationId")
            .IsUnique();
    }

    public DbSet<Car> Cars => Set<Car>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<Station> Stations => Set<Station>();
    public DbSet<Train> Trains => Set<Train>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserSession> UserSessions => Set<UserSession>();
    public DbSet<CarOperation> CarOperations => Set<CarOperation>();
    public DbSet<Freight> Freights => Set<Freight>();
    public DbSet<WayPoint> WayPoints => Set<WayPoint>();
}