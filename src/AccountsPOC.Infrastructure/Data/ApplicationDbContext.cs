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
    public DbSet<StockItem> StockItems => Set<StockItem>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<PriceList> PriceLists => Set<PriceList>();
    public DbSet<PriceListItem> PriceListItems => Set<PriceListItem>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<SystemSettings> SystemSettings => Set<SystemSettings>();
    public DbSet<CustomField> CustomFields => Set<CustomField>();
    public DbSet<CustomFieldValue> CustomFieldValues => Set<CustomFieldValue>();
    public DbSet<EmailTemplate> EmailTemplates => Set<EmailTemplate>();
    public DbSet<PaymentTransaction> PaymentTransactions => Set<PaymentTransaction>();
    public DbSet<PaymentProviderConfig> PaymentProviderConfigs => Set<PaymentProviderConfig>();
    public DbSet<ApiKeyConfig> ApiKeyConfigs => Set<ApiKeyConfig>();
    public DbSet<ConfigurationSetting> ConfigurationSettings => Set<ConfigurationSetting>();
    public DbSet<ExchangeRate> ExchangeRates => Set<ExchangeRate>();
    public DbSet<CustomForm> CustomForms => Set<CustomForm>();
    public DbSet<FormSubmission> FormSubmissions => Set<FormSubmission>();

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
            entity.HasIndex(e => e.TenantId);
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
            entity.HasIndex(e => e.TenantId);
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
            entity.HasIndex(e => e.TenantId);
            
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
            entity.HasIndex(e => e.TenantId);
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
            entity.HasIndex(e => e.TenantId);
            
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
        
        // StockItem configuration
        modelBuilder.Entity<StockItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.StockCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(200);
            entity.Property(e => e.LongDescription).HasMaxLength(1000);
            entity.Property(e => e.CostPrice).HasPrecision(18, 2);
            entity.Property(e => e.SellingPrice).HasPrecision(18, 2);
            entity.Property(e => e.BinLocation).HasMaxLength(50);
            entity.HasIndex(e => e.StockCode).IsUnique();
            entity.HasIndex(e => e.TenantId);
            
            entity.HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.Warehouse)
                .WithMany(e => e.StockItems)
                .HasForeignKey(e => e.WarehouseId)
                .OnDelete(DeleteBehavior.SetNull);
        });
        
        // Warehouse configuration
        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.WarehouseCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.WarehouseName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Address).IsRequired().HasMaxLength(500);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.PostCode).HasMaxLength(20);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.ContactName).HasMaxLength(200);
            entity.Property(e => e.ContactPhone).HasMaxLength(50);
            entity.HasIndex(e => e.WarehouseCode).IsUnique();
            entity.HasIndex(e => e.TenantId);
        });
        
        // PriceList configuration
        modelBuilder.Entity<PriceList>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PriceListCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Currency).HasMaxLength(10);
            entity.HasIndex(e => e.PriceListCode).IsUnique();
            entity.HasIndex(e => e.TenantId);
            
            entity.HasMany(e => e.PriceListItems)
                .WithOne(e => e.PriceList)
                .HasForeignKey(e => e.PriceListId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // PriceListItem configuration
        modelBuilder.Entity<PriceListItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Price).HasPrecision(18, 2);
            entity.Property(e => e.DiscountPercentage).HasPrecision(5, 2);
            
            entity.HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        // Customer configuration
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CustomerCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CompanyName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ContactName).HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.Address).IsRequired().HasMaxLength(500);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.PostCode).HasMaxLength(20);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.VATNumber).HasMaxLength(50);
            entity.Property(e => e.CreditLimit).HasPrecision(18, 2);
            entity.Property(e => e.CurrentBalance).HasPrecision(18, 2);
            entity.HasIndex(e => e.CustomerCode).IsUnique();
            entity.HasIndex(e => e.TenantId);
            
            entity.HasOne(e => e.DefaultPriceList)
                .WithMany()
                .HasForeignKey(e => e.DefaultPriceListId)
                .OnDelete(DeleteBehavior.SetNull);
        });
        
        // Tenant configuration
        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TenantCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CompanyName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ContactEmail).HasMaxLength(200);
            entity.Property(e => e.ContactPhone).HasMaxLength(50);
            entity.HasIndex(e => e.TenantCode).IsUnique();
        });
        
        // SystemSettings configuration
        modelBuilder.Entity<SystemSettings>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DefaultCurrency).IsRequired().HasMaxLength(10);
            entity.Property(e => e.CurrencySymbol).HasMaxLength(10);
            entity.Property(e => e.DateFormat).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CompanyName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.CompanyLogo).HasMaxLength(500);
            entity.Property(e => e.CompanyAddress).HasMaxLength(500);
            entity.Property(e => e.TaxNumber).HasMaxLength(50);
            entity.Property(e => e.DefaultTaxRate).HasPrecision(5, 2);
            entity.Property(e => e.EmailFromAddress).HasMaxLength(200);
            entity.Property(e => e.EmailFromName).HasMaxLength(200);
            entity.Property(e => e.StripePublishableKey).HasMaxLength(500);
            entity.Property(e => e.StripeSecretKey).HasMaxLength(500);
            entity.HasIndex(e => e.TenantId);
            
            entity.HasOne(e => e.Tenant)
                .WithMany()
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // CustomField configuration
        modelBuilder.Entity<CustomField>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EntityType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.FieldName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.FieldLabel).IsRequired().HasMaxLength(200);
            entity.Property(e => e.FieldType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Options).HasMaxLength(2000);
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => new { e.TenantId, e.EntityType });
            
            entity.HasOne(e => e.Tenant)
                .WithMany()
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // CustomFieldValue configuration
        modelBuilder.Entity<CustomFieldValue>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EntityType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.FieldValue).HasMaxLength(2000);
            entity.HasIndex(e => new { e.EntityType, e.EntityId });
            entity.HasIndex(e => e.CustomFieldId);
            
            entity.HasOne(e => e.CustomField)
                .WithMany()
                .HasForeignKey(e => e.CustomFieldId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // EmailTemplate configuration
        modelBuilder.Entity<EmailTemplate>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TemplateName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.TemplateCode).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Subject).IsRequired().HasMaxLength(500);
            entity.Property(e => e.BodyHtml).IsRequired();
            entity.Property(e => e.TriggerEvent).HasMaxLength(100);
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => new { e.TenantId, e.TemplateCode });
            
            entity.HasOne(e => e.Tenant)
                .WithMany()
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // PaymentTransaction configuration
        modelBuilder.Entity<PaymentTransaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PaymentMethod).IsRequired().HasMaxLength(50);
            entity.Property(e => e.TransactionId).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.Property(e => e.Currency).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CustomerEmail).HasMaxLength(200);
            entity.Property(e => e.PaymentIntentId).HasMaxLength(200);
            entity.Property(e => e.FailureReason).HasMaxLength(1000);
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => e.SalesInvoiceId);
            entity.HasIndex(e => e.TransactionId).IsUnique();
            
            entity.HasOne(e => e.Tenant)
                .WithMany()
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.SalesInvoice)
                .WithMany()
                .HasForeignKey(e => e.SalesInvoiceId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        // PaymentProviderConfig configuration
        modelBuilder.Entity<PaymentProviderConfig>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProviderName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ProviderCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PublishableKey).HasMaxLength(500);
            entity.Property(e => e.SecretKey).HasMaxLength(500);
            entity.Property(e => e.ApiKey).HasMaxLength(500);
            entity.Property(e => e.MerchantId).HasMaxLength(200);
            entity.Property(e => e.WebhookSecret).HasMaxLength(500);
            entity.Property(e => e.Environment).HasMaxLength(50);
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => new { e.TenantId, e.ProviderCode }).IsUnique();
            
            entity.HasOne(e => e.Tenant)
                .WithMany()
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        // ApiKeyConfig configuration
        modelBuilder.Entity<ApiKeyConfig>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ServiceName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.KeyName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.KeyType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.KeyValue).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Environment).HasMaxLength(50);
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => new { e.TenantId, e.ServiceName, e.KeyName });
            
            entity.HasOne(e => e.Tenant)
                .WithMany()
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        // ConfigurationSetting configuration (Generic Settings)
        modelBuilder.Entity<ConfigurationSetting>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Category).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Key).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Value).IsRequired();
            entity.Property(e => e.DataType).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.ValidationRule).HasMaxLength(500);
            entity.Property(e => e.DefaultValue).HasMaxLength(2000);
            entity.Property(e => e.LastModifiedBy).HasMaxLength(200);
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => new { e.TenantId, e.Category });
            entity.HasIndex(e => new { e.TenantId, e.Category, e.Key }).IsUnique();
            
            entity.HasOne(e => e.Tenant)
                .WithMany()
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        // ExchangeRate configuration
        modelBuilder.Entity<ExchangeRate>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FromCurrency).IsRequired().HasMaxLength(10);
            entity.Property(e => e.ToCurrency).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Rate).HasPrecision(18, 6);
            entity.Property(e => e.Source).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => new { e.TenantId, e.FromCurrency, e.ToCurrency, e.RateDate });
            entity.HasIndex(e => new { e.TenantId, e.FromCurrency, e.ToCurrency, e.IsActive });
        });
        
        // CustomForm configuration
        modelBuilder.Entity<CustomForm>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.FormFieldsJson).IsRequired().HasColumnType("TEXT");
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => new { e.TenantId, e.IsActive });
            
            entity.HasMany(e => e.Submissions)
                .WithOne(e => e.CustomForm)
                .HasForeignKey(e => e.CustomFormId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // FormSubmission configuration
        modelBuilder.Entity<FormSubmission>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AnswersJson).IsRequired().HasColumnType("TEXT");
            entity.Property(e => e.FileUploadsJson).HasColumnType("TEXT");
            entity.Property(e => e.SubmitterEmail).HasMaxLength(200);
            entity.Property(e => e.SubmitterIpAddress).HasMaxLength(50);
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => new { e.TenantId, e.CustomFormId });
            entity.HasIndex(e => e.SubmittedDate);
        });
    }
}
