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
                EmergencyContactName = "Mary Driver",
                EmergencyContactPhone = "+1234567899",
                VehicleRegistration = "VAN-001",
                VehicleType = "Delivery Van",
                VehicleCapacity = "500kg / 100 parcels",
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
                EmergencyContactName = "Robert Transport",
                EmergencyContactPhone = "+1234567898",
                VehicleRegistration = "VAN-002",
                VehicleType = "Box Truck",
                VehicleCapacity = "1000kg / 200 parcels",
                IsActive = true,
                EmploymentStartDate = DateTime.UtcNow.AddMonths(-6),
                CreatedDate = DateTime.UtcNow
            },
            new Driver
            {
                TenantId = tenant.Id,
                DriverCode = "DRV-003",
                FirstName = "Mike",
                LastName = "Courier",
                Email = "mike.courier@fulfillment.com",
                Phone = "+1234567896",
                MobilePhone = "+1987654324",
                LicenseNumber = "DL345678",
                LicenseExpiryDate = DateTime.UtcNow.AddYears(1).AddMonths(6),
                EmergencyContactName = "Lisa Courier",
                EmergencyContactPhone = "+1234567897",
                VehicleRegistration = "VAN-003",
                VehicleType = "Cargo Van",
                VehicleCapacity = "750kg / 150 parcels",
                IsActive = true,
                EmploymentStartDate = DateTime.UtcNow.AddMonths(-3),
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
                Status = DeliveryStopStatus.Pending,
                SafePlace = "Front Porch",
                BuildingAccessInstructions = "Ring doorbell at rear entrance",
                RequiresAgeVerification = false,
                DoorNumber = "Suite 200",
                ParcelCount = 3
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
                Status = DeliveryStopStatus.Pending,
                DoorAccessCode = "#1234",
                BuildingAccessInstructions = "Use loading dock entrance, press buzzer",
                RequiresAgeVerification = true,
                DoorNumber = "Dock 3",
                ParcelCount = 8
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
                Status = DeliveryStopStatus.Pending,
                SafePlace = "Behind Garage",
                DoorAccessCode = "*5B#",
                BuildingAccessInstructions = "Use intercom #5B",
                RequiresAgeVerification = false,
                DoorNumber = "5B",
                ParcelCount = 2
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
                Status = DeliveryStopStatus.Pending,
                PostBoxCode = "BOX789",
                BuildingAccessInstructions = "Business hours only. Back entrance to stockroom",
                RequiresAgeVerification = false,
                DoorNumber = "B1",
                ParcelCount = 5
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
                Status = DeliveryStopStatus.Pending,
                SafePlace = "Behind Garage",
                DoorAccessCode = "*5B#",
                RequiresAgeVerification = false,
                DoorNumber = "5B",
                ParcelCount = 4
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
    }
}
