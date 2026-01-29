using AccountsPOC.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AccountsPOC.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<User, Role, int, Microsoft.AspNetCore.Identity.IdentityUserClaim<int>, 
    UserRole, Microsoft.AspNetCore.Identity.IdentityUserLogin<int>, 
    Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>, Microsoft.AspNetCore.Identity.IdentityUserToken<int>>
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
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<PurchaseOrderItem> PurchaseOrderItems => Set<PurchaseOrderItem>();
    public DbSet<BrandingAsset> BrandingAssets => Set<BrandingAsset>();
    public DbSet<PickList> PickLists => Set<PickList>();
    public DbSet<PickListItem> PickListItems => Set<PickListItem>();
    public DbSet<StockCount> StockCounts => Set<StockCount>();
    public DbSet<StockCountItem> StockCountItems => Set<StockCountItem>();
    public DbSet<DeliveryRoute> DeliveryRoutes => Set<DeliveryRoute>();
    public DbSet<DeliveryStop> DeliveryStops => Set<DeliveryStop>();
    public DbSet<Driver> Drivers => Set<Driver>();
    public DbSet<Parcel> Parcels => Set<Parcel>();
    public DbSet<Container> Containers => Set<Container>();
    public DbSet<SalesInvoiceItem> SalesInvoiceItems => Set<SalesInvoiceItem>();
    public DbSet<PartialDispatch> PartialDispatches => Set<PartialDispatch>();
    public DbSet<PartialDispatchItem> PartialDispatchItems => Set<PartialDispatchItem>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<StockItemImage> StockItemImages => Set<StockItemImage>();
    public DbSet<License> Licenses => Set<License>();
    public DbSet<Installation> Installations => Set<Installation>();

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
            
            // Delivery-specific fields
            entity.Property(e => e.DeliveryAddress).HasMaxLength(500);
            entity.Property(e => e.DeliveryCity).HasMaxLength(100);
            entity.Property(e => e.DeliveryPostCode).HasMaxLength(20);
            entity.Property(e => e.DeliveryCountry).HasMaxLength(100);
            entity.Property(e => e.DeliveryContactName).HasMaxLength(200);
            entity.Property(e => e.DeliveryContactPhone).HasMaxLength(50);
            entity.Property(e => e.DeliveryContactMobile).HasMaxLength(50);
            entity.Property(e => e.DeliveryInstructions).HasMaxLength(1000);
            entity.Property(e => e.PreferredDeliveryTime).HasMaxLength(100);
            entity.Property(e => e.AccessCode).HasMaxLength(50);
            
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
        
        // Supplier configuration
        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SupplierCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.SupplierName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ContactName).HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.PostalCode).HasMaxLength(20);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.WebsiteUrl).HasMaxLength(500);
            entity.Property(e => e.ApiEndpoint).HasMaxLength(500);
            entity.Property(e => e.ApiUsername).HasMaxLength(100);
            entity.Property(e => e.ApiPassword).HasMaxLength(100);
            entity.Property(e => e.PaymentTerms).HasMaxLength(200);
            entity.Property(e => e.MinimumOrderValue).HasPrecision(18, 2);
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => new { e.TenantId, e.SupplierCode }).IsUnique();
            
            entity.HasOne(e => e.Tenant)
                .WithMany()
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        // PurchaseOrder configuration
        modelBuilder.Entity<PurchaseOrder>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.SubTotal).HasPrecision(18, 2);
            entity.Property(e => e.TaxAmount).HasPrecision(18, 2);
            entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
            entity.Property(e => e.Notes).HasMaxLength(2000);
            entity.Property(e => e.DeliveryAddress).HasMaxLength(500);
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => new { e.TenantId, e.OrderNumber }).IsUnique();
            entity.HasIndex(e => e.SupplierId);
            
            entity.HasOne(e => e.Tenant)
                .WithMany()
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.Supplier)
                .WithMany()
                .HasForeignKey(e => e.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasMany(e => e.Items)
                .WithOne(e => e.PurchaseOrder)
                .HasForeignKey(e => e.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // PurchaseOrderItem configuration
        modelBuilder.Entity<PurchaseOrderItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
            entity.Property(e => e.Discount).HasPrecision(18, 2);
            entity.Property(e => e.TaxRate).HasPrecision(18, 2);
            entity.Property(e => e.LineTotal).HasPrecision(18, 2);
            entity.HasIndex(e => e.PurchaseOrderId);
            entity.HasIndex(e => e.ProductId);
            
            entity.HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        // BrandingAsset configuration
        modelBuilder.Entity<BrandingAsset>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AssetType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.AssetName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.MimeType).HasMaxLength(100);
            entity.Property(e => e.AltText).HasMaxLength(500);
            entity.Property(e => e.UsageContext).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.LastModifiedBy).HasMaxLength(100);
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => new { e.TenantId, e.AssetType, e.IsActive });
        });
        
        // PickList configuration
        modelBuilder.Entity<PickList>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PickListNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.AssignedToUserName).HasMaxLength(200);
            entity.Property(e => e.Notes).HasMaxLength(2000);
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => new { e.TenantId, e.PickListNumber }).IsUnique();
            
            entity.HasOne(e => e.SalesOrder)
                .WithMany()
                .HasForeignKey(e => e.SalesOrderId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasMany(e => e.Items)
                .WithOne(e => e.PickList)
                .HasForeignKey(e => e.PickListId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // PickListItem configuration
        modelBuilder.Entity<PickListItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BinLocation).HasMaxLength(50);
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.HasIndex(e => e.PickListId);
            entity.HasIndex(e => e.StockItemId);
            
            entity.HasOne(e => e.StockItem)
                .WithMany()
                .HasForeignKey(e => e.StockItemId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        // StockCount configuration
        modelBuilder.Entity<StockCount>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CountNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CountedByUserName).HasMaxLength(200);
            entity.Property(e => e.Notes).HasMaxLength(2000);
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => new { e.TenantId, e.CountNumber }).IsUnique();
            
            entity.HasOne(e => e.Warehouse)
                .WithMany()
                .HasForeignKey(e => e.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasMany(e => e.Items)
                .WithOne(e => e.StockCount)
                .HasForeignKey(e => e.StockCountId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // StockCountItem configuration
        modelBuilder.Entity<StockCountItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.HasIndex(e => e.StockCountId);
            entity.HasIndex(e => e.StockItemId);
            
            entity.HasOne(e => e.StockItem)
                .WithMany()
                .HasForeignKey(e => e.StockItemId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        // DeliveryRoute configuration
        modelBuilder.Entity<DeliveryRoute>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RouteNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.VehicleRegistration).HasMaxLength(50);
            entity.Property(e => e.Notes).HasMaxLength(2000);
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => new { e.TenantId, e.RouteNumber }).IsUnique();
            
            entity.HasOne(e => e.Driver)
                .WithMany(d => d.DeliveryRoutes)
                .HasForeignKey(e => e.DriverId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasMany(e => e.Stops)
                .WithOne(e => e.DeliveryRoute)
                .HasForeignKey(e => e.DeliveryRouteId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // DeliveryStop configuration
        modelBuilder.Entity<DeliveryStop>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DeliveryAddress).IsRequired().HasMaxLength(500);
            entity.Property(e => e.ContactName).HasMaxLength(200);
            entity.Property(e => e.ContactPhone).HasMaxLength(50);
            entity.Property(e => e.ContactEmail).HasMaxLength(200);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.DeliveryNotes).HasMaxLength(2000);
            entity.Property(e => e.SignatureImagePath).HasMaxLength(500);
            entity.Property(e => e.PhotoEvidencePaths).HasMaxLength(2000);
            
            // Safe place and access configuration
            entity.Property(e => e.SafePlace).HasMaxLength(200);
            entity.Property(e => e.DoorAccessCode).HasMaxLength(50);
            entity.Property(e => e.PostBoxCode).HasMaxLength(50);
            entity.Property(e => e.BuildingAccessInstructions).HasMaxLength(1000);
            
            // OTP configuration
            entity.Property(e => e.OTPCode).HasMaxLength(10);
            
            entity.HasIndex(e => e.DeliveryRouteId);
            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => e.SalesOrderId);
            
            entity.HasOne(e => e.Customer)
                .WithMany()
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.SalesOrder)
                .WithMany()
                .HasForeignKey(e => e.SalesOrderId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        // Driver configuration
        modelBuilder.Entity<Driver>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DriverCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Phone).IsRequired().HasMaxLength(50);
            entity.Property(e => e.MobilePhone).HasMaxLength(50);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.PostCode).HasMaxLength(20);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.LicenseNumber).HasMaxLength(50);
            entity.Property(e => e.EmergencyContactName).HasMaxLength(200);
            entity.Property(e => e.EmergencyContactPhone).HasMaxLength(50);
            entity.Property(e => e.VehicleRegistration).HasMaxLength(50);
            entity.Property(e => e.VehicleType).HasMaxLength(100);
            entity.Property(e => e.VehicleCapacity).HasMaxLength(100);
            entity.Property(e => e.Notes).HasMaxLength(2000);
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => new { e.TenantId, e.DriverCode }).IsUnique();
            entity.HasIndex(e => e.Email);
        });
        
        // Parcel configuration
        modelBuilder.Entity<Parcel>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ParcelBarcode).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => e.ParcelBarcode).IsUnique();
            entity.HasIndex(e => e.DeliveryStopId);
            entity.HasIndex(e => e.ContainerId);
            
            entity.HasOne(e => e.SalesOrder)
                .WithMany()
                .HasForeignKey(e => e.SalesOrderId)
                .OnDelete(DeleteBehavior.SetNull);
                
            entity.HasOne(e => e.DeliveryStop)
                .WithMany()
                .HasForeignKey(e => e.DeliveryStopId)
                .OnDelete(DeleteBehavior.SetNull);
                
            entity.HasOne(e => e.Container)
                .WithMany(e => e.Parcels)
                .HasForeignKey(e => e.ContainerId)
                .OnDelete(DeleteBehavior.SetNull);
                
            entity.HasOne(e => e.ScannedByDriver)
                .WithMany()
                .HasForeignKey(e => e.ScannedByDriverId)
                .OnDelete(DeleteBehavior.SetNull);
        });
        
        // Container configuration
        modelBuilder.Entity<Container>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ContainerCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ContainerType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => new { e.TenantId, e.ContainerCode }).IsUnique();
            entity.HasIndex(e => e.DeliveryRouteId);
            
            entity.HasOne(e => e.DeliveryRoute)
                .WithMany()
                .HasForeignKey(e => e.DeliveryRouteId)
                .OnDelete(DeleteBehavior.SetNull);
                
            entity.HasOne(e => e.Driver)
                .WithMany()
                .HasForeignKey(e => e.DriverId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // User configuration (ASP.NET Identity)
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.HasIndex(e => e.TenantId);
        });

        // Role configuration (ASP.NET Identity)
        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        // Permission configuration
        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Resource).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Action).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => new { e.Resource, e.Action }).IsUnique();
        });

        // UserRole configuration (ASP.NET Identity)
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(ur => new { ur.UserId, ur.RoleId });
            
            entity.HasOne(e => e.User)
                .WithMany(e => e.UserRoles)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.Role)
                .WithMany(e => e.UserRoles)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // RolePermission configuration
        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.RoleId, e.PermissionId }).IsUnique();
            
            entity.HasOne(e => e.Role)
                .WithMany(e => e.RolePermissions)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.Permission)
                .WithMany(e => e.RolePermissions)
                .HasForeignKey(e => e.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // StockItemImage configuration
        modelBuilder.Entity<StockItemImage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ImageUrl).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Caption).HasMaxLength(200);
            entity.HasIndex(e => e.StockItemId);
            
            entity.HasOne(e => e.StockItem)
                .WithMany()
                .HasForeignKey(e => e.StockItemId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // License configuration
        modelBuilder.Entity<License>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.LicenseKey).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LicenseType).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.LicenseKey).IsUnique();
            entity.HasIndex(e => e.TenantId);
            
            entity.HasOne(e => e.Tenant)
                .WithMany()
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Installation configuration
        modelBuilder.Entity<Installation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.InstallationKey).IsRequired().HasMaxLength(100);
            entity.Property(e => e.MachineName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.MachineIdentifier).HasMaxLength(200);
            entity.Property(e => e.IpAddress).HasMaxLength(45);
            entity.Property(e => e.Version).HasMaxLength(50);
            entity.HasIndex(e => e.InstallationKey).IsUnique();
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => e.LicenseId);
            
            entity.HasOne(e => e.Tenant)
                .WithMany()
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.License)
                .WithMany()
                .HasForeignKey(e => e.LicenseId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
