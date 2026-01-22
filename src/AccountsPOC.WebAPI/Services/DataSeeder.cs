using AccountsPOC.Domain.Entities;
using AccountsPOC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AccountsPOC.WebAPI.Services;

public class DataSeeder
{
    private readonly ApplicationDbContext _context;

    public DataSeeder(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        // Ensure database is created
        await _context.Database.EnsureCreatedAsync();

        // Check if data already exists
        if (await _context.Warehouses.AnyAsync())
        {
            return; // Database already seeded
        }

        // Seed Tenant
        var tenant = new Tenant
        {
            TenantCode = "DEMO",
            CompanyName = "Demo Fulfillment Co.",
            ContactEmail = "demo@fulfillment.com",
            ContactPhone = "+1234567890",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };
        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync();

        // Seed Warehouses
        var warehouse = new Warehouse
        {
            TenantId = tenant.Id,
            WarehouseCode = "WH-01",
            WarehouseName = "Main Distribution Center",
            Address = "123 Warehouse Lane",
            City = "Distribution City",
            PostCode = "12345",
            Country = "USA",
            ContactName = "Warehouse Manager",
            ContactPhone = "+1234567891",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };
        _context.Warehouses.Add(warehouse);
        await _context.SaveChangesAsync();

        // Seed Drivers
        var drivers = new List<Driver>
        {
            new Driver
            {
                TenantId = tenant.Id,
                DriverCode = "DRV-001",
                FirstName = "John",
                LastName = "Driver",
                Email = "john.driver@fulfillment.com",
                Phone = "+1234567892",
                MobilePhone = "+1987654321",
                LicenseNumber = "DL123456",
                LicenseExpiryDate = DateTime.UtcNow.AddYears(2),
                VehicleRegistration = "VAN-001",
                VehicleType = "Delivery Van",
                VehicleCapacity = "500kg",
                IsActive = true,
                EmploymentStartDate = DateTime.UtcNow.AddYears(-1),
                CreatedDate = DateTime.UtcNow
            },
            new Driver
            {
                TenantId = tenant.Id,
                DriverCode = "DRV-002",
                FirstName = "Jane",
                LastName = "Transport",
                Email = "jane.transport@fulfillment.com",
                Phone = "+1234567893",
                MobilePhone = "+1987654322",
                LicenseNumber = "DL789012",
                LicenseExpiryDate = DateTime.UtcNow.AddYears(3),
                VehicleRegistration = "VAN-002",
                VehicleType = "Box Truck",
                VehicleCapacity = "1000kg",
                IsActive = true,
                EmploymentStartDate = DateTime.UtcNow.AddMonths(-6),
                CreatedDate = DateTime.UtcNow
            }
        };
        _context.Drivers.AddRange(drivers);
        await _context.SaveChangesAsync();

        // Seed Customers
        var customers = new List<Customer>
        {
            new Customer
            {
                TenantId = tenant.Id,
                CustomerCode = "CUST-001",
                CompanyName = "Tech Solutions Inc",
                ContactName = "Bob Johnson",
                Email = "bob@techsolutions.com",
                Phone = "+1234567894",
                Address = "456 Business Ave",
                City = "Tech City",
                PostCode = "54321",
                Country = "USA",
                DeliveryAddress = "456 Business Ave, Rear Entrance",
                DeliveryCity = "Tech City",
                DeliveryPostCode = "54321",
                DeliveryCountry = "USA",
                DeliveryContactPhone = "+1234567894",
                DeliveryContactMobile = "+1987654323",
                DeliveryInstructions = "Ring doorbell. Safe place: Porch",
                DeliveryLatitude = 40.7128,
                DeliveryLongitude = -74.0060,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            },
            new Customer
            {
                TenantId = tenant.Id,
                CustomerCode = "CUST-002",
                CompanyName = "Retail Warehouse",
                ContactName = "Alice Smith",
                Email = "alice@retailwarehouse.com",
                Phone = "+1234567895",
                Address = "789 Retail Blvd",
                City = "Commerce City",
                PostCode = "67890",
                Country = "USA",
                DeliveryAddress = "789 Retail Blvd, Loading Dock",
                DeliveryCity = "Commerce City",
                DeliveryPostCode = "67890",
                DeliveryCountry = "USA",
                DeliveryContactPhone = "+1234567895",
                DeliveryInstructions = "Deliver to loading dock. Access code: #1234",
                PreferredDeliveryTime = "8:00 AM - 12:00 PM",
                AccessCode = "#1234",
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            }
        };
        _context.Customers.AddRange(customers);
        await _context.SaveChangesAsync();

        // Seed Products
        var products = new List<Product>
        {
            new Product
            {
                TenantId = tenant.Id,
                ProductCode = "PROD-001",
                ProductName = "Standard Widget",
                Description = "A standard widget for general use",
                UnitPrice = 19.99m,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            },
            new Product
            {
                TenantId = tenant.Id,
                ProductCode = "PROD-002",
                ProductName = "Premium Gadget",
                Description = "A premium gadget with advanced features",
                UnitPrice = 49.99m,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            },
            new Product
            {
                TenantId = tenant.Id,
                ProductCode = "PROD-AGE",
                ProductName = "Age-Restricted Item",
                Description = "Requires age verification (18+)",
                UnitPrice = 29.99m,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            }
        };
        _context.Products.AddRange(products);
        await _context.SaveChangesAsync();

        // Seed Stock Items
        var stockItems = new List<StockItem>
        {
            new StockItem
            {
                TenantId = tenant.Id,
                StockCode = "STK-001",
                Description = "Standard Widget",
                ProductId = products[0].Id,
                WarehouseId = warehouse.Id,
                CostPrice = 15.00m,
                SellingPrice = 19.99m,
                QuantityOnHand = 100,
                QuantityAllocated = 10,
                ReorderLevel = 20,
                ReorderQuantity = 50,
                BinLocation = "A1-B2-C3",
                IsActive = true,
                IsAgeRestricted = false,
                CreatedDate = DateTime.UtcNow
            },
            new StockItem
            {
                TenantId = tenant.Id,
                StockCode = "STK-002",
                Description = "Premium Gadget",
                ProductId = products[1].Id,
                WarehouseId = warehouse.Id,
                CostPrice = 35.00m,
                SellingPrice = 49.99m,
                QuantityOnHand = 50,
                QuantityAllocated = 5,
                ReorderLevel = 15,
                ReorderQuantity = 30,
                BinLocation = "A2-B1-C2",
                IsActive = true,
                IsAgeRestricted = false,
                CreatedDate = DateTime.UtcNow
            },
            new StockItem
            {
                TenantId = tenant.Id,
                StockCode = "STK-AGE",
                Description = "Age-Restricted Item (18+)",
                ProductId = products[2].Id,
                WarehouseId = warehouse.Id,
                CostPrice = 20.00m,
                SellingPrice = 29.99m,
                QuantityOnHand = 30,
                QuantityAllocated = 3,
                ReorderLevel = 10,
                ReorderQuantity = 20,
                BinLocation = "SECURE-A1",
                IsActive = true,
                IsAgeRestricted = true,
                MinimumAge = 18,
                RequiresOTPVerification = true,
                CreatedDate = DateTime.UtcNow
            }
        };
        _context.StockItems.AddRange(stockItems);
        await _context.SaveChangesAsync();

        // Seed Delivery Route
        var route = new DeliveryRoute
        {
            TenantId = tenant.Id,
            RouteNumber = "ROUTE-001",
            RouteDate = DateTime.Today,
            Status = "Planned",
            DriverId = drivers[0].Id,
            VehicleRegistration = "VAN-001",
            CreatedDate = DateTime.UtcNow
        };
        _context.DeliveryRoutes.Add(route);
        await _context.SaveChangesAsync();

        // Seed Delivery Stops
        var stops = new List<DeliveryStop>
        {
            new DeliveryStop
            {
                DeliveryRouteId = route.Id,
                SequenceNumber = 1,
                CustomerId = customers[0].Id,
                DeliveryAddress = "456 Business Ave, Rear Entrance",
                ContactName = "Bob Johnson",
                ContactPhone = "+1234567894",
                ContactEmail = "bob@techsolutions.com",
                Latitude = 40.7128,
                Longitude = -74.0060,
                Status = "Pending",
                SafePlace = "Porch",
                RequiresAgeVerification = false
            },
            new DeliveryStop
            {
                DeliveryRouteId = route.Id,
                SequenceNumber = 2,
                CustomerId = customers[1].Id,
                DeliveryAddress = "789 Retail Blvd, Loading Dock",
                ContactName = "Alice Smith",
                ContactPhone = "+1234567895",
                Latitude = 40.7589,
                Longitude = -73.9851,
                Status = "Pending",
                DoorAccessCode = "#1234",
                BuildingAccessInstructions = "Use loading dock entrance",
                RequiresAgeVerification = true
            }
        };
        _context.DeliveryStops.AddRange(stops);
        await _context.SaveChangesAsync();
    }
}
