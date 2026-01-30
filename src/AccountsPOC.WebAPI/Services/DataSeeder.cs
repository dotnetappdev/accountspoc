using AccountsPOC.Domain.Entities;
using AccountsPOC.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AccountsPOC.WebAPI.Services;

public class DataSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;

    public DataSeeder(ApplicationDbContext context, UserManager<User> userManager, RoleManager<Role> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    /// <summary>
    /// Seeds only basic data (tenants, warehouses, drivers, etc.) without users
    /// </summary>
    public async Task SeedBasicDataAsync()
    {
        // Ensure database is created
        await _context.Database.EnsureCreatedAsync();

        // Check if data already exists
        if (await _context.Warehouses.AnyAsync())
        {
            return; // Database already seeded with basic data
        }

        await SeedTenantAndWarehousesAsync();
    }

    /// <summary>
    /// Seeds the tenant and warehouses
    /// </summary>
    private async Task<(Tenant tenant, Warehouse warehouse, List<Driver> drivers)> SeedTenantAndWarehousesAsync()
    {
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

        // Seed Warehouses and related data
        var (warehouse, drivers) = await SeedWarehousesAndDriversAsync(tenant.Id);
        
        return (tenant, warehouse, drivers);
    }

    private async Task<(Warehouse warehouse, List<Driver> drivers)> SeedWarehousesAndDriversAsync(int tenantId)
    {
        // Seed Warehouses
        var warehouse = new Warehouse
        {
            TenantId = tenantId,
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
                TenantId = tenantId,
                DriverCode = "DRV-001",
                FirstName = "John",
                LastName = "Driver",
                Email = "john.driver@fulfillment.com",
                Phone = "+1234567892",
                MobilePhone = "+1987654321",
                LicenseNumber = "DL123456",
                LicenseExpiryDate = DateTime.UtcNow.AddYears(2),
                EmergencyContactName = "Mary Driver",
                EmergencyContactPhone = "+1234567899",
                VehicleRegistration = "VAN-001",
                VehicleType = "Delivery Van",
                VehicleCapacity = "500kg / 100 parcels",
                IsActive = true,
                EmploymentStartDate = DateTime.UtcNow.AddYears(-1),
                CreatedDate = DateTime.UtcNow
            }
        };
        _context.Drivers.AddRange(drivers);
        await _context.SaveChangesAsync();
        
        return (warehouse, drivers);
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

        // Seed basic data first
        var (tenant, warehouse, drivers) = await SeedTenantAndWarehousesAsync();

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
                DeliveryInstructions = "Ring doorbell at rear entrance. Safe place: Front Porch if no answer",
                DeliveryLatitude = 40.7128,
                DeliveryLongitude = -74.0060,
                PreferredDeliveryTime = "9:00 AM - 5:00 PM",
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
                DeliveryInstructions = "Deliver to loading dock at rear. Press buzzer for access",
                DeliveryLatitude = 40.7589,
                DeliveryLongitude = -73.9851,
                PreferredDeliveryTime = "8:00 AM - 12:00 PM",
                AccessCode = "#1234|LOAD5678",
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            },
            new Customer
            {
                TenantId = tenant.Id,
                CustomerCode = "CUST-003",
                CompanyName = "Home Office - Sarah Williams",
                ContactName = "Sarah Williams",
                Email = "sarah.williams@email.com",
                Phone = "+1234567900",
                Address = "321 Residential St",
                City = "Suburbia",
                PostCode = "11223",
                Country = "USA",
                DeliveryAddress = "321 Residential St, Apt 5B",
                DeliveryCity = "Suburbia",
                DeliveryPostCode = "11223",
                DeliveryCountry = "USA",
                DeliveryContactPhone = "+1234567900",
                DeliveryContactMobile = "+1987654325",
                DeliveryInstructions = "Apartment building - use intercom #5B. Safe place: Behind garage if no answer",
                DeliveryLatitude = 40.7306,
                DeliveryLongitude = -73.9352,
                PreferredDeliveryTime = "12:00 PM - 6:00 PM",
                AccessCode = "*5B#",
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            },
            new Customer
            {
                TenantId = tenant.Id,
                CustomerCode = "CUST-004",
                CompanyName = "Green Grocery Market",
                ContactName = "Tom Green",
                Email = "tom@greengrocery.com",
                Phone = "+1234567901",
                Address = "555 Market Square",
                City = "Downtown",
                PostCode = "33445",
                Country = "USA",
                DeliveryAddress = "555 Market Square, Back Entrance",
                DeliveryCity = "Downtown",
                DeliveryPostCode = "33445",
                DeliveryCountry = "USA",
                DeliveryContactPhone = "+1234567901",
                DeliveryContactMobile = "+1987654326",
                DeliveryInstructions = "Business hours only. Deliver to stockroom via back entrance",
                DeliveryLatitude = 40.7580,
                DeliveryLongitude = -73.9855,
                PreferredDeliveryTime = "6:00 AM - 10:00 AM",
                AccessCode = "BOX789",
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
            },
            new Product
            {
                TenantId = tenant.Id,
                ProductCode = "PROD-003",
                ProductName = "Office Supplies Bundle",
                Description = "Complete office supplies package",
                UnitPrice = 79.99m,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            },
            new Product
            {
                TenantId = tenant.Id,
                ProductCode = "PROD-004",
                ProductName = "Electronics Kit",
                Description = "Electronic components starter kit",
                UnitPrice = 149.99m,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            },
            new Product
            {
                TenantId = tenant.Id,
                ProductCode = "PROD-005",
                ProductName = "Home Appliance",
                Description = "Household appliance item",
                UnitPrice = 299.99m,
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
                QuantityOnHand = 250,
                QuantityAllocated = 25,
                ReorderLevel = 50,
                ReorderQuantity = 100,
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
                QuantityOnHand = 180,
                QuantityAllocated = 15,
                ReorderLevel = 30,
                ReorderQuantity = 60,
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
                QuantityOnHand = 75,
                QuantityAllocated = 8,
                ReorderLevel = 20,
                ReorderQuantity = 40,
                BinLocation = "SECURE-A1",
                IsActive = true,
                IsAgeRestricted = true,
                MinimumAge = 18,
                RequiresOTPVerification = true,
                CreatedDate = DateTime.UtcNow
            },
            new StockItem
            {
                TenantId = tenant.Id,
                StockCode = "STK-003",
                Description = "Office Supplies Bundle",
                ProductId = products[3].Id,
                WarehouseId = warehouse.Id,
                CostPrice = 55.00m,
                SellingPrice = 79.99m,
                QuantityOnHand = 120,
                QuantityAllocated = 12,
                ReorderLevel = 25,
                ReorderQuantity = 50,
                BinLocation = "B1-C2-D3",
                IsActive = true,
                IsAgeRestricted = false,
                CreatedDate = DateTime.UtcNow
            },
            new StockItem
            {
                TenantId = tenant.Id,
                StockCode = "STK-004",
                Description = "Electronics Kit",
                ProductId = products[4].Id,
                WarehouseId = warehouse.Id,
                CostPrice = 110.00m,
                SellingPrice = 149.99m,
                QuantityOnHand = 65,
                QuantityAllocated = 7,
                ReorderLevel = 15,
                ReorderQuantity = 30,
                BinLocation = "C1-D2-E3",
                IsActive = true,
                IsAgeRestricted = false,
                CreatedDate = DateTime.UtcNow
            },
            new StockItem
            {
                TenantId = tenant.Id,
                StockCode = "STK-005",
                Description = "Home Appliance",
                ProductId = products[5].Id,
                WarehouseId = warehouse.Id,
                CostPrice = 220.00m,
                SellingPrice = 299.99m,
                QuantityOnHand = 40,
                QuantityAllocated = 5,
                ReorderLevel = 10,
                ReorderQuantity = 20,
                BinLocation = "D1-E2-F3",
                IsActive = true,
                IsAgeRestricted = false,
                CreatedDate = DateTime.UtcNow
            }
        };
        _context.StockItems.AddRange(stockItems);
        await _context.SaveChangesAsync();

        // Seed Delivery Route
        var routes = new List<DeliveryRoute>
        {
            new DeliveryRoute
            {
                TenantId = tenant.Id,
                RouteNumber = "ROUTE-001",
                RouteDate = DateTime.Today,
                Status = "Planned",
                DriverId = drivers[0].Id,
                VehicleRegistration = "VAN-001",
                CreatedDate = DateTime.UtcNow
            },
            new DeliveryRoute
            {
                TenantId = tenant.Id,
                RouteNumber = "ROUTE-002",
                RouteDate = DateTime.Today,
                Status = "Planned",
                DriverId = drivers[1].Id,
                VehicleRegistration = "VAN-002",
                CreatedDate = DateTime.UtcNow
            }
        };
        _context.DeliveryRoutes.AddRange(routes);
        await _context.SaveChangesAsync();

        // Seed Delivery Stops
        var stops = new List<DeliveryStop>
        {
            // Route 1 stops
            new DeliveryStop
            {
                DeliveryRouteId = routes[0].Id,
                SequenceNumber = 1,
                CustomerId = customers[0].Id,
                DeliveryAddress = "456 Business Ave, Rear Entrance",
                ContactName = "Bob Johnson",
                ContactPhone = "+1234567894",
                ContactEmail = "bob@techsolutions.com",
                Latitude = 40.7128,
                Longitude = -74.0060,
                Status = "Pending",
                SafePlace = "Front Porch",
                BuildingAccessInstructions = "Ring doorbell at rear entrance",
                RequiresAgeVerification = false
            },
            new DeliveryStop
            {
                DeliveryRouteId = routes[0].Id,
                SequenceNumber = 2,
                CustomerId = customers[1].Id,
                DeliveryAddress = "789 Retail Blvd, Loading Dock",
                ContactName = "Alice Smith",
                ContactPhone = "+1234567895",
                Latitude = 40.7589,
                Longitude = -73.9851,
                Status = "Pending",
                DoorAccessCode = "#1234",
                BuildingAccessInstructions = "Use loading dock entrance, press buzzer",
                RequiresAgeVerification = true
            },
            new DeliveryStop
            {
                DeliveryRouteId = routes[0].Id,
                SequenceNumber = 3,
                CustomerId = customers[2].Id,
                DeliveryAddress = "321 Residential St, Apt 5B",
                ContactName = "Sarah Williams",
                ContactPhone = "+1234567900",
                ContactEmail = "sarah.williams@email.com",
                Latitude = 40.7306,
                Longitude = -73.9352,
                Status = "Pending",
                SafePlace = "Behind Garage",
                DoorAccessCode = "*5B#",
                BuildingAccessInstructions = "Use intercom #5B",
                RequiresAgeVerification = false
            },
            // Route 2 stops
            new DeliveryStop
            {
                DeliveryRouteId = routes[1].Id,
                SequenceNumber = 1,
                CustomerId = customers[3].Id,
                DeliveryAddress = "555 Market Square, Back Entrance",
                ContactName = "Tom Green",
                ContactPhone = "+1234567901",
                ContactEmail = "deliveries@greengrocery.com",
                Latitude = 40.7580,
                Longitude = -73.9855,
                Status = "Pending",
                PostBoxCode = "BOX789",
                BuildingAccessInstructions = "Business hours only. Back entrance to stockroom",
                RequiresAgeVerification = false
            },
            new DeliveryStop
            {
                DeliveryRouteId = routes[1].Id,
                SequenceNumber = 2,
                CustomerId = customers[2].Id,
                DeliveryAddress = "321 Residential St, Apt 5B",
                ContactName = "Sarah Williams",
                ContactPhone = "+1234567900",
                Latitude = 40.7306,
                Longitude = -73.9352,
                Status = "Pending",
                SafePlace = "Behind Garage",
                DoorAccessCode = "*5B#",
                RequiresAgeVerification = false
            }
        };
        _context.DeliveryStops.AddRange(stops);
        await _context.SaveChangesAsync();

        // Seed Containers
        var containers = new List<Container>
        {
            new Container
            {
                TenantId = tenant.Id,
                ContainerCode = "BAG-001",
                ContainerType = "Bag",
                MaxCapacity = 15,
                Status = "Available",
                CreatedDate = DateTime.UtcNow
            },
            new Container
            {
                TenantId = tenant.Id,
                ContainerCode = "BAG-002",
                ContainerType = "Bag",
                MaxCapacity = 15,
                Status = "Available",
                CreatedDate = DateTime.UtcNow
            },
            new Container
            {
                TenantId = tenant.Id,
                ContainerCode = "CAGE-001",
                ContainerType = "Cage",
                MaxCapacity = 50,
                Status = "Available",
                CreatedDate = DateTime.UtcNow
            },
            new Container
            {
                TenantId = tenant.Id,
                ContainerCode = "CAGE-002",
                ContainerType = "Cage",
                MaxCapacity = 50,
                Status = "Available",
                CreatedDate = DateTime.UtcNow
            },
            new Container
            {
                TenantId = tenant.Id,
                ContainerCode = "TROLLEY-001",
                ContainerType = "Trolley",
                MaxCapacity = 30,
                Status = "Available",
                CreatedDate = DateTime.UtcNow
            }
        };
        _context.Containers.AddRange(containers);
        await _context.SaveChangesAsync();

        // Seed Parcels
        var parcels = new List<Parcel>
        {
            new Parcel
            {
                TenantId = tenant.Id,
                ParcelBarcode = "PKG-001-2026",
                DeliveryStopId = stops[0].Id,
                Status = "Pending",
                CreatedDate = DateTime.UtcNow
            },
            new Parcel
            {
                TenantId = tenant.Id,
                ParcelBarcode = "PKG-002-2026",
                DeliveryStopId = stops[0].Id,
                Status = "Pending",
                CreatedDate = DateTime.UtcNow
            },
            new Parcel
            {
                TenantId = tenant.Id,
                ParcelBarcode = "PKG-003-2026",
                DeliveryStopId = stops[1].Id,
                Status = "Pending",
                CreatedDate = DateTime.UtcNow
            },
            new Parcel
            {
                TenantId = tenant.Id,
                ParcelBarcode = "PKG-004-2026",
                DeliveryStopId = stops[1].Id,
                Status = "Pending",
                CreatedDate = DateTime.UtcNow
            },
            new Parcel
            {
                TenantId = tenant.Id,
                ParcelBarcode = "PKG-005-2026",
                DeliveryStopId = stops[2].Id,
                Status = "Pending",
                CreatedDate = DateTime.UtcNow
            },
            new Parcel
            {
                TenantId = tenant.Id,
                ParcelBarcode = "PKG-006-2026",
                DeliveryStopId = stops[3].Id,
                Status = "Pending",
                CreatedDate = DateTime.UtcNow
            },
            new Parcel
            {
                TenantId = tenant.Id,
                ParcelBarcode = "PKG-007-2026",
                DeliveryStopId = stops[3].Id,
                Status = "Pending",
                CreatedDate = DateTime.UtcNow
            },
            new Parcel
            {
                TenantId = tenant.Id,
                ParcelBarcode = "PKG-008-2026",
                DeliveryStopId = stops[4].Id,
                Status = "Pending",
                CreatedDate = DateTime.UtcNow
            }
        };
        _context.Parcels.AddRange(parcels);
        await _context.SaveChangesAsync();

        // Seed Roles and Permissions
        await SeedRolesAndPermissions(tenant.Id);
    }

    private async Task SeedRolesAndPermissions(int tenantId)
    {
        // Check if permissions already exist
        if (await _context.Permissions.AnyAsync())
        {
            return; // Already seeded
        }

        // Create comprehensive accounting permissions
        var permissions = new List<Permission>
        {
            // Tenant permissions
            new Permission { Name = "Create Tenant", Resource = "Tenant", Action = "Create", Description = "Can create new tenants", CreatedDate = DateTime.UtcNow },
            new Permission { Name = "Read Tenant", Resource = "Tenant", Action = "Read", Description = "Can view tenant information", CreatedDate = DateTime.UtcNow },
            new Permission { Name = "Update Tenant", Resource = "Tenant", Action = "Update", Description = "Can update tenant information", CreatedDate = DateTime.UtcNow },
            new Permission { Name = "Delete Tenant", Resource = "Tenant", Action = "Delete", Description = "Can delete tenants", CreatedDate = DateTime.UtcNow },
            
            // Customer permissions
            new Permission { Name = "Create Customer", Resource = "Customer", Action = "Create", Description = "Can create new customers", CreatedDate = DateTime.UtcNow },
            new Permission { Name = "Read Customer", Resource = "Customer", Action = "Read", Description = "Can view customer information", CreatedDate = DateTime.UtcNow },
            new Permission { Name = "Update Customer", Resource = "Customer", Action = "Update", Description = "Can update customer information", CreatedDate = DateTime.UtcNow },
            new Permission { Name = "Delete Customer", Resource = "Customer", Action = "Delete", Description = "Can delete customers", CreatedDate = DateTime.UtcNow },
            new Permission { Name = "Manage Customer Contacts", Resource = "Customer", Action = "Manage", Description = "Can manage customer contact and delivery information", CreatedDate = DateTime.UtcNow },
            
            // Stock Item permissions
            new Permission { Name = "Create Stock Item", Resource = "StockItem", Action = "Create", Description = "Can create new stock items", CreatedDate = DateTime.UtcNow },
            new Permission { Name = "Read Stock Item", Resource = "StockItem", Action = "Read", Description = "Can view stock items", CreatedDate = DateTime.UtcNow },
            new Permission { Name = "Update Stock Item", Resource = "StockItem", Action = "Update", Description = "Can update stock items", CreatedDate = DateTime.UtcNow },
            new Permission { Name = "Delete Stock Item", Resource = "StockItem", Action = "Delete", Description = "Can delete stock items", CreatedDate = DateTime.UtcNow },
            new Permission { Name = "Manage Stock Images", Resource = "StockItem", Action = "Manage", Description = "Can manage stock item images", CreatedDate = DateTime.UtcNow },
            
            // Invoice & Billing permissions
            new Permission { Name = "Create Invoice", Resource = "Invoice", Action = "Create", Description = "Can create sales invoices", CreatedDate = DateTime.UtcNow },
            new Permission { Name = "Read Invoice", Resource = "Invoice", Action = "Read", Description = "Can view invoices", CreatedDate = DateTime.UtcNow },
            new Permission { Name = "Update Invoice", Resource = "Invoice", Action = "Update", Description = "Can update invoices", CreatedDate = DateTime.UtcNow },
            new Permission { Name = "Delete Invoice", Resource = "Invoice", Action = "Delete", Description = "Can delete invoices", CreatedDate = DateTime.UtcNow },
            new Permission { Name = "Approve Invoice", Resource = "Invoice", Action = "Approve", Description = "Can approve invoices for posting", CreatedDate = DateTime.UtcNow },
            new Permission { Name = "Post Invoice", Resource = "Invoice", Action = "Post", Description = "Can post invoices to ledger", CreatedDate = DateTime.UtcNow },
            
            // Purchase Order permissions
            new Permission { Name = "Create Purchase Order", Resource = "PurchaseOrder", Action = "Create", Description = "Can create purchase orders", CreatedDate = DateTime.UtcNow },
            new Permission { Name = "Read Purchase Order", Resource = "PurchaseOrder", Action = "Read", Description = "Can view purchase orders", CreatedDate = DateTime.UtcNow },
            new Permission { Name = "Update Purchase Order", Resource = "PurchaseOrder", Action = "Update", Description = "Can update purchase orders", CreatedDate = DateTime.UtcNow },
            new Permission { Name = "Delete Purchase Order", Resource = "PurchaseOrder", Action = "Delete", Description = "Can delete purchase orders", CreatedDate = DateTime.UtcNow },
            new Permission { Name = "Approve Purchase Order", Resource = "PurchaseOrder", Action = "Approve", Description = "Can approve purchase orders", CreatedDate = DateTime.UtcNow },
            
            // Financial Reports permissions
            new Permission { Name = "View Financial Reports", Resource = "Reports", Action = "Read", Description = "Can view financial reports", CreatedDate = DateTime.UtcNow },
            new Permission { Name = "Export Financial Reports", Resource = "Reports", Action = "Export", Description = "Can export financial reports", CreatedDate = DateTime.UtcNow },
            new Permission { Name = "View Management Reports", Resource = "Reports", Action = "ViewManagement", Description = "Can view management-level reports", CreatedDate = DateTime.UtcNow },
            
            // Bank Account permissions
            new Permission { Name = "Create Bank Account", Resource = "BankAccount", Action = "Create", Description = "Can create bank accounts", CreatedDate = DateTime.UtcNow },
            new Permission { Name = "Read Bank Account", Resource = "BankAccount", Action = "Read", Description = "Can view bank accounts", CreatedDate = DateTime.UtcNow },
            new Permission { Name = "Update Bank Account", Resource = "BankAccount", Action = "Update", Description = "Can update bank accounts", CreatedDate = DateTime.UtcNow },
            new Permission { Name = "Delete Bank Account", Resource = "BankAccount", Action = "Delete", Description = "Can delete bank accounts", CreatedDate = DateTime.UtcNow },
            new Permission { Name = "Reconcile Bank Account", Resource = "BankAccount", Action = "Reconcile", Description = "Can reconcile bank accounts", CreatedDate = DateTime.UtcNow },
            
            // Payment permissions
            new Permission { Name = "Process Payment", Resource = "Payment", Action = "Process", Description = "Can process payments", CreatedDate = DateTime.UtcNow },
            new Permission { Name = "Approve Payment", Resource = "Payment", Action = "Approve", Description = "Can approve payments", CreatedDate = DateTime.UtcNow },
            new Permission { Name = "View Payment History", Resource = "Payment", Action = "Read", Description = "Can view payment history", CreatedDate = DateTime.UtcNow },
            
            // User management permissions
            new Permission { Name = "Create User", Resource = "User", Action = "Create", Description = "Can create users", CreatedDate = DateTime.UtcNow },
            new Permission { Name = "Read User", Resource = "User", Action = "Read", Description = "Can view users", CreatedDate = DateTime.UtcNow },
            new Permission { Name = "Update User", Resource = "User", Action = "Update", Description = "Can update users", CreatedDate = DateTime.UtcNow },
            new Permission { Name = "Delete User", Resource = "User", Action = "Delete", Description = "Can delete users", CreatedDate = DateTime.UtcNow },
            new Permission { Name = "Manage Roles", Resource = "Role", Action = "Manage", Description = "Can manage roles", CreatedDate = DateTime.UtcNow },
            new Permission { Name = "Manage Permissions", Resource = "Permission", Action = "Manage", Description = "Can manage permissions", CreatedDate = DateTime.UtcNow },
            
            // System Settings permissions
            new Permission { Name = "Manage System Settings", Resource = "SystemSettings", Action = "Manage", Description = "Can manage system settings", CreatedDate = DateTime.UtcNow },
            new Permission { Name = "View Audit Logs", Resource = "AuditLog", Action = "Read", Description = "Can view audit logs", CreatedDate = DateTime.UtcNow }
        };
        _context.Permissions.AddRange(permissions);
        await _context.SaveChangesAsync();

        // Create accounting-focused roles using RoleManager
        
        // 1. Super Administrator - Full system control
        var superAdminRole = new Role
        {
            Name = "Super Administrator",
            Description = "Highest level access with full system control and configuration",
            IsSystemRole = true,
            CreatedDate = DateTime.UtcNow
        };
        await _roleManager.CreateAsync(superAdminRole);
        
        // 2. Administrator - Full operational access
        var adminRole = new Role
        {
            Name = "Administrator",
            Description = "Full operational access to all modules",
            IsSystemRole = true,
            CreatedDate = DateTime.UtcNow
        };
        await _roleManager.CreateAsync(adminRole);
        
        // 3. Financial Controller - Senior financial management
        var financialControllerRole = new Role
        {
            Name = "Financial Controller",
            Description = "Senior financial management with approval authority",
            IsSystemRole = true,
            CreatedDate = DateTime.UtcNow
        };
        await _roleManager.CreateAsync(financialControllerRole);
        
        // 4. Accounting Manager - Daily accounting operations
        var accountingManagerRole = new Role
        {
            Name = "Accounting Manager",
            Description = "Manages daily accounting operations and team",
            IsSystemRole = true,
            CreatedDate = DateTime.UtcNow
        };
        await _roleManager.CreateAsync(accountingManagerRole);
        
        // 5. Senior Accountant - Complex transactions
        var seniorAccountantRole = new Role
        {
            Name = "Senior Accountant",
            Description = "Handles complex transactions and reconciliations",
            IsSystemRole = true,
            CreatedDate = DateTime.UtcNow
        };
        await _roleManager.CreateAsync(seniorAccountantRole);
        
        // 6. Accountant - Standard accounting tasks
        var accountantRole = new Role
        {
            Name = "Accountant",
            Description = "Performs standard accounting tasks and data entry",
            IsSystemRole = true,
            CreatedDate = DateTime.UtcNow
        };
        await _roleManager.CreateAsync(accountantRole);
        
        // 7. Bookkeeper - Basic data entry
        var bookkeeperRole = new Role
        {
            Name = "Bookkeeper",
            Description = "Basic data entry and record maintenance",
            IsSystemRole = true,
            CreatedDate = DateTime.UtcNow
        };
        await _roleManager.CreateAsync(bookkeeperRole);
        
        // 8. Accounts Payable Clerk
        var apClerkRole = new Role
        {
            Name = "Accounts Payable Clerk",
            Description = "Manages vendor invoices and payments",
            IsSystemRole = true,
            CreatedDate = DateTime.UtcNow
        };
        await _roleManager.CreateAsync(apClerkRole);
        
        // 9. Accounts Receivable Clerk
        var arClerkRole = new Role
        {
            Name = "Accounts Receivable Clerk",
            Description = "Manages customer invoices and collections",
            IsSystemRole = true,
            CreatedDate = DateTime.UtcNow
        };
        await _roleManager.CreateAsync(arClerkRole);
        
        // 10. Payroll Manager
        var payrollManagerRole = new Role
        {
            Name = "Payroll Manager",
            Description = "Manages payroll processing and compliance",
            IsSystemRole = true,
            CreatedDate = DateTime.UtcNow
        };
        await _roleManager.CreateAsync(payrollManagerRole);
        
        // 11. Support - Customer support
        var supportRole = new Role
        {
            Name = "Support",
            Description = "Customer support with tenant and customer management",
            IsSystemRole = true,
            CreatedDate = DateTime.UtcNow
        };
        await _roleManager.CreateAsync(supportRole);

        // 12. Agent - Field operations
        var agentRole = new Role
        {
            Name = "Agent",
            Description = "Field agents with limited customer access",
            IsSystemRole = true,
            CreatedDate = DateTime.UtcNow
        };
        await _roleManager.CreateAsync(agentRole);

        // 13. User - Standard user
        var userRole = new Role
        {
            Name = "User",
            Description = "Standard user with basic read access",
            IsSystemRole = true,
            CreatedDate = DateTime.UtcNow
        };
        await _roleManager.CreateAsync(userRole);
        
        // 14. Auditor - Read-only financial access
        var auditorRole = new Role
        {
            Name = "Auditor",
            Description = "Read-only access to financial records and audit logs",
            IsSystemRole = true,
            CreatedDate = DateTime.UtcNow
        };
        await _roleManager.CreateAsync(auditorRole);

        // Assign permissions to roles based on hierarchy
        
        // Super Administrator - ALL permissions
        foreach (var perm in permissions)
        {
            _context.RolePermissions.Add(new RolePermission
            {
                RoleId = superAdminRole.Id,
                PermissionId = perm.Id,
                AssignedDate = DateTime.UtcNow
            });
        }
        
        // Administrator - All except system settings
        var adminPermissions = permissions.Where(p => p.Resource != "SystemSettings" && p.Resource != "AuditLog").ToList();
        foreach (var perm in adminPermissions)
        {
            _context.RolePermissions.Add(new RolePermission
            {
                RoleId = adminRole.Id,
                PermissionId = perm.Id,
                AssignedDate = DateTime.UtcNow
            });
        }
        
        // Financial Controller - All financial operations with approval
        var fcPermissions = permissions.Where(p => 
            p.Resource == "Invoice" || 
            p.Resource == "PurchaseOrder" || 
            p.Resource == "Payment" || 
            p.Resource == "BankAccount" || 
            p.Resource == "Reports" || 
            p.Resource == "Customer" ||
            p.Resource == "StockItem").ToList();
        foreach (var perm in fcPermissions)
        {
            _context.RolePermissions.Add(new RolePermission
            {
                RoleId = financialControllerRole.Id,
                PermissionId = perm.Id,
                AssignedDate = DateTime.UtcNow
            });
        }
        
        // Accounting Manager - Financial ops without highest approvals
        var amPermissions = permissions.Where(p => 
            (p.Resource == "Invoice" && p.Action != "Delete") || 
            (p.Resource == "PurchaseOrder" && p.Action != "Delete") || 
            (p.Resource == "Payment" && p.Action == "Process") || 
            (p.Resource == "BankAccount" && p.Action != "Delete") ||
            p.Resource == "Reports" ||
            (p.Resource == "Customer" && p.Action == "Read") ||
            (p.Resource == "StockItem" && p.Action == "Read")).ToList();
        foreach (var perm in amPermissions)
        {
            _context.RolePermissions.Add(new RolePermission
            {
                RoleId = accountingManagerRole.Id,
                PermissionId = perm.Id,
                AssignedDate = DateTime.UtcNow
            });
        }
        
        // Senior Accountant - Complex transactions
        var saPermissions = permissions.Where(p => 
            (p.Resource == "Invoice" && p.Action != "Delete" && p.Action != "Approve") || 
            (p.Resource == "BankAccount" && p.Action == "Reconcile") ||
            (p.Resource == "BankAccount" && p.Action == "Read") ||
            (p.Resource == "Reports" && p.Action == "Read")).ToList();
        foreach (var perm in saPermissions)
        {
            _context.RolePermissions.Add(new RolePermission
            {
                RoleId = seniorAccountantRole.Id,
                PermissionId = perm.Id,
                AssignedDate = DateTime.UtcNow
            });
        }
        
        // Accountant - Standard accounting
        var accountantPermissions = permissions.Where(p => 
            (p.Resource == "Invoice" && (p.Action == "Create" || p.Action == "Read" || p.Action == "Update")) ||
            (p.Resource == "Customer" && p.Action == "Read") ||
            (p.Resource == "Reports" && p.Action == "Read")).ToList();
        foreach (var perm in accountantPermissions)
        {
            _context.RolePermissions.Add(new RolePermission
            {
                RoleId = accountantRole.Id,
                PermissionId = perm.Id,
                AssignedDate = DateTime.UtcNow
            });
        }
        
        // Bookkeeper - Basic data entry
        var bookkeeperPermissions = permissions.Where(p => 
            (p.Resource == "Invoice" && (p.Action == "Create" || p.Action == "Read")) ||
            (p.Resource == "Customer" && p.Action == "Read")).ToList();
        foreach (var perm in bookkeeperPermissions)
        {
            _context.RolePermissions.Add(new RolePermission
            {
                RoleId = bookkeeperRole.Id,
                PermissionId = perm.Id,
                AssignedDate = DateTime.UtcNow
            });
        }
        
        // AP Clerk - Vendor and payment focus
        var apPermissions = permissions.Where(p => 
            (p.Resource == "PurchaseOrder" && p.Action != "Delete" && p.Action != "Approve") ||
            (p.Resource == "Payment" && p.Action == "Process")).ToList();
        foreach (var perm in apPermissions)
        {
            _context.RolePermissions.Add(new RolePermission
            {
                RoleId = apClerkRole.Id,
                PermissionId = perm.Id,
                AssignedDate = DateTime.UtcNow
            });
        }
        
        // AR Clerk - Customer invoice focus
        var arPermissions = permissions.Where(p => 
            (p.Resource == "Invoice" && p.Action != "Delete" && p.Action != "Approve") ||
            (p.Resource == "Customer" && (p.Action == "Read" || p.Action == "Update"))).ToList();
        foreach (var perm in arPermissions)
        {
            _context.RolePermissions.Add(new RolePermission
            {
                RoleId = arClerkRole.Id,
                PermissionId = perm.Id,
                AssignedDate = DateTime.UtcNow
            });
        }
        
        // Payroll Manager - Payroll specific (placeholder for now)
        var payrollPermissions = permissions.Where(p => 
            p.Resource == "Reports" && p.Action == "Read").ToList();
        foreach (var perm in payrollPermissions)
        {
            _context.RolePermissions.Add(new RolePermission
            {
                RoleId = payrollManagerRole.Id,
                PermissionId = perm.Id,
                AssignedDate = DateTime.UtcNow
            });
        }
        
        // Support - Tenant and customer management
        var supportPermissions = permissions.Where(p => 
            p.Resource == "Tenant" || 
            p.Resource == "Customer").ToList();
        foreach (var perm in supportPermissions)
        {
            _context.RolePermissions.Add(new RolePermission
            {
                RoleId = supportRole.Id,
                PermissionId = perm.Id,
                AssignedDate = DateTime.UtcNow
            });
        }

        // Agent - Limited customer management
        var agentPermissions = permissions.Where(p => 
            p.Resource == "Customer" && (p.Action == "Read" || p.Action == "Manage")).ToList();
        foreach (var perm in agentPermissions)
        {
            _context.RolePermissions.Add(new RolePermission
            {
                RoleId = agentRole.Id,
                PermissionId = perm.Id,
                AssignedDate = DateTime.UtcNow
            });
        }
        
        // Auditor - Read-only financial access
        var auditorPermissions = permissions.Where(p => 
            p.Action == "Read" || p.Resource == "AuditLog").ToList();
        foreach (var perm in auditorPermissions)
        {
            _context.RolePermissions.Add(new RolePermission
            {
                RoleId = auditorRole.Id,
                PermissionId = perm.Id,
                AssignedDate = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync();

        // Create comprehensive user accounts for accounting department
        
        // Super Administrators (2)
        var superAdmin1 = new User
        {
            TenantId = tenantId,
            UserName = "superadmin",
            Email = "superadmin@accountspoc.com",
            FirstName = "System",
            LastName = "Administrator",
            PhoneNumber = "+1-555-0100",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };
        await _userManager.CreateAsync(superAdmin1, "SuperAdmin123!");
        await _userManager.AddToRoleAsync(superAdmin1, "Super Administrator");
        
        var superAdmin2 = new User
        {
            TenantId = tenantId,
            UserName = "sysadmin",
            Email = "sysadmin@accountspoc.com",
            FirstName = "IT",
            LastName = "Administrator",
            PhoneNumber = "+1-555-0101",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };
        await _userManager.CreateAsync(superAdmin2, "SysAdmin123!");
        await _userManager.AddToRoleAsync(superAdmin2, "Super Administrator");
        
        // Administrators (3)
        var admin1 = new User
        {
            TenantId = tenantId,
            UserName = "admin",
            Email = "admin@accountspoc.com",
            FirstName = "John",
            LastName = "Administrator",
            PhoneNumber = "+1-555-0102",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };
        await _userManager.CreateAsync(admin1, "Admin123!");
        await _userManager.AddToRoleAsync(admin1, "Administrator");
        
        var admin2 = new User
        {
            TenantId = tenantId,
            UserName = "admin.backup",
            Email = "admin.backup@accountspoc.com",
            FirstName = "Sarah",
            LastName = "Williams",
            PhoneNumber = "+1-555-0103",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };
        await _userManager.CreateAsync(admin2, "Admin123!");
        await _userManager.AddToRoleAsync(admin2, "Administrator");
        
        var admin3 = new User
        {
            TenantId = tenantId,
            UserName = "ops.admin",
            Email = "ops.admin@accountspoc.com",
            FirstName = "Michael",
            LastName = "Operations",
            PhoneNumber = "+1-555-0104",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };
        await _userManager.CreateAsync(admin3, "Admin123!");
        await _userManager.AddToRoleAsync(admin3, "Administrator");
        
        // Financial Controllers (2)
        var fc1 = new User
        {
            TenantId = tenantId,
            UserName = "robert.controller",
            Email = "robert.controller@accountspoc.com",
            FirstName = "Robert",
            LastName = "Harrison",
            PhoneNumber = "+1-555-0110",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };
        await _userManager.CreateAsync(fc1, "FinCtrl123!");
        await _userManager.AddToRoleAsync(fc1, "Financial Controller");
        
        var fc2 = new User
        {
            TenantId = tenantId,
            UserName = "jennifer.cfo",
            Email = "jennifer.cfo@accountspoc.com",
            FirstName = "Jennifer",
            LastName = "Martinez",
            PhoneNumber = "+1-555-0111",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };
        await _userManager.CreateAsync(fc2, "FinCtrl123!");
        await _userManager.AddToRoleAsync(fc2, "Financial Controller");
        
        // Accounting Managers (2)
        var am1 = new User
        {
            TenantId = tenantId,
            UserName = "david.manager",
            Email = "david.manager@accountspoc.com",
            FirstName = "David",
            LastName = "Thompson",
            PhoneNumber = "+1-555-0120",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };
        await _userManager.CreateAsync(am1, "AcctMgr123!");
        await _userManager.AddToRoleAsync(am1, "Accounting Manager");
        
        var am2 = new User
        {
            TenantId = tenantId,
            UserName = "lisa.accounting",
            Email = "lisa.accounting@accountspoc.com",
            FirstName = "Lisa",
            LastName = "Anderson",
            PhoneNumber = "+1-555-0121",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };
        await _userManager.CreateAsync(am2, "AcctMgr123!");
        await _userManager.AddToRoleAsync(am2, "Accounting Manager");
        
        // Senior Accountants (3)
        var sa1 = new User
        {
            TenantId = tenantId,
            UserName = "james.senior",
            Email = "james.senior@accountspoc.com",
            FirstName = "James",
            LastName = "Wilson",
            PhoneNumber = "+1-555-0130",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };
        await _userManager.CreateAsync(sa1, "SrAcct123!");
        await _userManager.AddToRoleAsync(sa1, "Senior Accountant");
        
        var sa2 = new User
        {
            TenantId = tenantId,
            UserName = "emily.senior",
            Email = "emily.senior@accountspoc.com",
            FirstName = "Emily",
            LastName = "Davis",
            PhoneNumber = "+1-555-0131",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };
        await _userManager.CreateAsync(sa2, "SrAcct123!");
        await _userManager.AddToRoleAsync(sa2, "Senior Accountant");
        
        var sa3 = new User
        {
            TenantId = tenantId,
            UserName = "william.senior",
            Email = "william.senior@accountspoc.com",
            FirstName = "William",
            LastName = "Brown",
            PhoneNumber = "+1-555-0132",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };
        await _userManager.CreateAsync(sa3, "SrAcct123!");
        await _userManager.AddToRoleAsync(sa3, "Senior Accountant");
        
        // Accountants (4)
        var acc1 = new User
        {
            TenantId = tenantId,
            UserName = "susan.accountant",
            Email = "susan.accountant@accountspoc.com",
            FirstName = "Susan",
            LastName = "Miller",
            PhoneNumber = "+1-555-0140",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };
        await _userManager.CreateAsync(acc1, "Acct123!");
        await _userManager.AddToRoleAsync(acc1, "Accountant");
        
        var acc2 = new User
        {
            TenantId = tenantId,
            UserName = "thomas.accountant",
            Email = "thomas.accountant@accountspoc.com",
            FirstName = "Thomas",
            LastName = "Moore",
            PhoneNumber = "+1-555-0141",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };
        await _userManager.CreateAsync(acc2, "Acct123!");
        await _userManager.AddToRoleAsync(acc2, "Accountant");
        
        var acc3 = new User
        {
            TenantId = tenantId,
            UserName = "patricia.accountant",
            Email = "patricia.accountant@accountspoc.com",
            FirstName = "Patricia",
            LastName = "Taylor",
            PhoneNumber = "+1-555-0142",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };
        await _userManager.CreateAsync(acc3, "Acct123!");
        await _userManager.AddToRoleAsync(acc3, "Accountant");
        
        var acc4 = new User
        {
            TenantId = tenantId,
            UserName = "daniel.accountant",
            Email = "daniel.accountant@accountspoc.com",
            FirstName = "Daniel",
            LastName = "Jackson",
            PhoneNumber = "+1-555-0143",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };
        await _userManager.CreateAsync(acc4, "Acct123!");
        await _userManager.AddToRoleAsync(acc4, "Accountant");
        
        // Bookkeepers (3)
        var bk1 = new User
        {
            TenantId = tenantId,
            UserName = "mary.bookkeeper",
            Email = "mary.bookkeeper@accountspoc.com",
            FirstName = "Mary",
            LastName = "White",
            PhoneNumber = "+1-555-0150",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };
        await _userManager.CreateAsync(bk1, "BookKpr123!");
        await _userManager.AddToRoleAsync(bk1, "Bookkeeper");
        
        var bk2 = new User
        {
            TenantId = tenantId,
            UserName = "karen.bookkeeper",
            Email = "karen.bookkeeper@accountspoc.com",
            FirstName = "Karen",
            LastName = "Harris",
            PhoneNumber = "+1-555-0151",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };
        await _userManager.CreateAsync(bk2, "BookKpr123!");
        await _userManager.AddToRoleAsync(bk2, "Bookkeeper");
        
        var bk3 = new User
        {
            TenantId = tenantId,
            UserName = "nancy.bookkeeper",
            Email = "nancy.bookkeeper@accountspoc.com",
            FirstName = "Nancy",
            LastName = "Clark",
            PhoneNumber = "+1-555-0152",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };
        await _userManager.CreateAsync(bk3, "BookKpr123!");
        await _userManager.AddToRoleAsync(bk3, "Bookkeeper");
        
        // AP Clerks (2)
        var ap1 = new User
        {
            TenantId = tenantId,
            UserName = "betty.payables",
            Email = "betty.payables@accountspoc.com",
            FirstName = "Betty",
            LastName = "Lewis",
            PhoneNumber = "+1-555-0160",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };
        await _userManager.CreateAsync(ap1, "APClerk123!");
        await _userManager.AddToRoleAsync(ap1, "Accounts Payable Clerk");
        
        var ap2 = new User
        {
            TenantId = tenantId,
            UserName = "helen.payables",
            Email = "helen.payables@accountspoc.com",
            FirstName = "Helen",
            LastName = "Robinson",
            PhoneNumber = "+1-555-0161",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };
        await _userManager.CreateAsync(ap2, "APClerk123!");
        await _userManager.AddToRoleAsync(ap2, "Accounts Payable Clerk");
        
        // AR Clerks (2)
        var ar1 = new User
        {
            TenantId = tenantId,
            UserName = "sandra.receivables",
            Email = "sandra.receivables@accountspoc.com",
            FirstName = "Sandra",
            LastName = "Walker",
            PhoneNumber = "+1-555-0170",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };
        await _userManager.CreateAsync(ar1, "ARClerk123!");
        await _userManager.AddToRoleAsync(ar1, "Accounts Receivable Clerk");
        
        var ar2 = new User
        {
            TenantId = tenantId,
            UserName = "donna.receivables",
            Email = "donna.receivables@accountspoc.com",
            FirstName = "Donna",
            LastName = "Young",
            PhoneNumber = "+1-555-0171",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };
        await _userManager.CreateAsync(ar2, "ARClerk123!");
        await _userManager.AddToRoleAsync(ar2, "Accounts Receivable Clerk");
        
        // Payroll Managers (1)
        var pr1 = new User
        {
            TenantId = tenantId,
            UserName = "paul.payroll",
            Email = "paul.payroll@accountspoc.com",
            FirstName = "Paul",
            LastName = "Allen",
            PhoneNumber = "+1-555-0180",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };
        await _userManager.CreateAsync(pr1, "PayRoll123!");
        await _userManager.AddToRoleAsync(pr1, "Payroll Manager");
        
        // Auditors (2)
        var aud1 = new User
        {
            TenantId = tenantId,
            UserName = "carol.auditor",
            Email = "carol.auditor@accountspoc.com",
            FirstName = "Carol",
            LastName = "King",
            PhoneNumber = "+1-555-0190",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };
        await _userManager.CreateAsync(aud1, "Audit123!");
        await _userManager.AddToRoleAsync(aud1, "Auditor");
        
        var aud2 = new User
        {
            TenantId = tenantId,
            UserName = "george.auditor",
            Email = "george.auditor@accountspoc.com",
            FirstName = "George",
            LastName = "Scott",
            PhoneNumber = "+1-555-0191",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };
        await _userManager.CreateAsync(aud2, "Audit123!");
        await _userManager.AddToRoleAsync(aud2, "Auditor");

        // Support Staff (2)
        var supportUser1 = new User
        {
            TenantId = tenantId,
            UserName = "support",
            Email = "support@accountspoc.com",
            FirstName = "Support",
            LastName = "Team",
            PhoneNumber = "+1-555-0200",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };
        await _userManager.CreateAsync(supportUser1, "Support123!");
        await _userManager.AddToRoleAsync(supportUser1, "Support");
        
        var supportUser2 = new User
        {
            TenantId = tenantId,
            UserName = "support.lead",
            Email = "support.lead@accountspoc.com",
            FirstName = "Jessica",
            LastName = "Support",
            PhoneNumber = "+1-555-0201",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };
        await _userManager.CreateAsync(supportUser2, "Support123!");
        await _userManager.AddToRoleAsync(supportUser2, "Support");

        // Field Agents (2)
        var agentUser1 = new User
        {
            TenantId = tenantId,
            UserName = "agent",
            Email = "agent@accountspoc.com",
            FirstName = "Field",
            LastName = "Agent",
            PhoneNumber = "+1-555-0210",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };
        await _userManager.CreateAsync(agentUser1, "Agent123!");
        await _userManager.AddToRoleAsync(agentUser1, "Agent");
        
        var agentUser2 = new User
        {
            TenantId = tenantId,
            UserName = "agent.mobile",
            Email = "agent.mobile@accountspoc.com",
            FirstName = "Mark",
            LastName = "Field",
            PhoneNumber = "+1-555-0211",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };
        await _userManager.CreateAsync(agentUser2, "Agent123!");
        await _userManager.AddToRoleAsync(agentUser2, "Agent");

        // Update UserRoles with AssignedDate
        var userRoles = await _context.UserRoles.ToListAsync();
        foreach (var ur in userRoles)
        {
            ur.AssignedDate = DateTime.UtcNow;
        }
        await _context.SaveChangesAsync();

        // Create default license for the tenant
        var defaultLicense = new License
        {
            TenantId = tenantId,
            LicenseKey = $"DEMO-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}",
            LicenseType = "Professional",
            IsActive = true,
            ActivationDate = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddYears(1),
            MaxInstallations = 5,
            MaxStockItems = 1000,
            AllowMultipleImages = true,
            MaxImagesPerStockItem = 10,
            MaxUsers = 50,
            MaxRoles = 20,
            MaxCustomers = 500,
            MaxTenants = 1,
            MaxSalesOrdersPerMonth = null, // Unlimited
            MaxPurchaseOrdersPerMonth = null, // Unlimited
            MaxWarehouses = 10,
            MaxProducts = 1000,
            EnablePdfExport = true,
            EnableEmailTemplates = true,
            EnableCustomForms = true,
            EnablePaymentIntegration = true,
            EnableAdvancedReporting = true,
            EnableApiAccess = true,
            EnableMultipleCurrencies = true,
            Notes = "Demo Professional License",
            CreatedDate = DateTime.UtcNow
        };
        _context.Licenses.Add(defaultLicense);
        await _context.SaveChangesAsync();
    }
}
