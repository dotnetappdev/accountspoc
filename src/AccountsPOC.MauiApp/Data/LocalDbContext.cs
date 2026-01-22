using Microsoft.EntityFrameworkCore;
using AccountsPOC.Domain.Entities;

namespace AccountsPOC.MauiApp.Data;

public class LocalDbContext : DbContext
{
    public LocalDbContext(DbContextOptions<LocalDbContext> options) : base(options)
    {
    }

    // Fulfillment entities for offline sync
    public DbSet<DeliveryRoute> DeliveryRoutes => Set<DeliveryRoute>();
    public DbSet<DeliveryStop> DeliveryStops => Set<DeliveryStop>();
    public DbSet<Driver> Drivers => Set<Driver>();
    public DbSet<StockItem> StockItems => Set<StockItem>();
    public DbSet<PickList> PickLists => Set<PickList>();
    public DbSet<PickListItem> PickListItems => Set<PickListItem>();
    public DbSet<StockCount> StockCounts => Set<StockCount>();
    public DbSet<StockCountItem> StockCountItems => Set<StockCountItem>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<Customer> Customers => Set<Customer>();
    
    // Sync tracking
    public DbSet<SyncLog> SyncLogs => Set<SyncLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure relationships for local database
        modelBuilder.Entity<DeliveryRoute>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasMany(e => e.Stops)
                .WithOne(e => e.DeliveryRoute)
                .HasForeignKey(e => e.DeliveryRouteId);
        });

        modelBuilder.Entity<PickList>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasMany(e => e.Items)
                .WithOne(e => e.PickList)
                .HasForeignKey(e => e.PickListId);
        });

        modelBuilder.Entity<StockCount>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasMany(e => e.Items)
                .WithOne(e => e.StockCount)
                .HasForeignKey(e => e.StockCountId);
        });

        modelBuilder.Entity<SyncLog>(entity =>
        {
            entity.HasKey(e => e.Id);
        });
    }
}

public class SyncLog
{
    public int Id { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public string Operation { get; set; } = string.Empty; // Create, Update, Delete
    public DateTime Timestamp { get; set; }
    public bool Synced { get; set; } = false;
    public DateTime? SyncedAt { get; set; }
    public string? SyncError { get; set; }
    public string? DataJson { get; set; } // Store the entity data for sync
}
