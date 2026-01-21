using AccountsPOC.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AccountsPOC.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<BankAccount> BankAccounts => Set<BankAccount>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<SalesOrder> SalesOrders => Set<SalesOrder>();
    public DbSet<SalesOrderItem> SalesOrderItems => Set<SalesOrderItem>();
    public DbSet<SalesInvoice> SalesInvoices => Set<SalesInvoice>();
    public DbSet<BillOfMaterial> BillOfMaterials => Set<BillOfMaterial>();
    public DbSet<BOMComponent> BOMComponents => Set<BOMComponent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // BankAccount configuration
        modelBuilder.Entity<BankAccount>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AccountName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.AccountNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.BankName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.BranchCode).HasMaxLength(50);
            entity.Property(e => e.Currency).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Balance).HasPrecision(18, 2);
        });

        // Product configuration
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProductCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ProductName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
            entity.HasIndex(e => e.ProductCode).IsUnique();
        });

        // SalesOrder configuration
        modelBuilder.Entity<SalesOrder>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CustomerName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.CustomerEmail).HasMaxLength(200);
            entity.Property(e => e.CustomerPhone).HasMaxLength(50);
            entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.OrderNumber).IsUnique();
            
            entity.HasMany(e => e.SalesOrderItems)
                .WithOne(e => e.SalesOrder)
                .HasForeignKey(e => e.SalesOrderId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasMany(e => e.SalesInvoices)
                .WithOne(e => e.SalesOrder)
                .HasForeignKey(e => e.SalesOrderId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // SalesOrderItem configuration
        modelBuilder.Entity<SalesOrderItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
            entity.Property(e => e.TotalPrice).HasPrecision(18, 2);
            
            entity.HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.BillOfMaterial)
                .WithMany(e => e.SalesOrderItems)
                .HasForeignKey(e => e.BillOfMaterialId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // SalesInvoice configuration
        modelBuilder.Entity<SalesInvoice>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.InvoiceNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.SubTotal).HasPrecision(18, 2);
            entity.Property(e => e.TaxAmount).HasPrecision(18, 2);
            entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.InvoiceNumber).IsUnique();
        });

        // BillOfMaterial configuration
        modelBuilder.Entity<BillOfMaterial>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BOMNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.EstimatedCost).HasPrecision(18, 2);
            entity.HasIndex(e => e.BOMNumber).IsUnique();
            
            entity.HasOne(e => e.Product)
                .WithMany(e => e.BillOfMaterials)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasMany(e => e.Components)
                .WithOne(e => e.BillOfMaterial)
                .HasForeignKey(e => e.BillOfMaterialId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // BOMComponent configuration
        modelBuilder.Entity<BOMComponent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UnitCost).HasPrecision(18, 2);
            entity.Property(e => e.TotalCost).HasPrecision(18, 2);
            
            entity.HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
