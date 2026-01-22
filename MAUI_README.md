# AccountsPOC - MAUI Fulfillment App

This solution now includes a .NET MAUI mobile application for warehouse and fulfillment operations, designed to work like an Amazon fulfillment app.

## Projects

### AccountsPOC.Domain
Core domain entities including new fulfillment entities:
- `PickList` & `PickListItem` - Pick list management
- `StockCount` & `StockCountItem` - Stock counting operations
- `DeliveryRoute` & `DeliveryStop` - Delivery route management with GPS coordinates

### AccountsPOC.Infrastructure
Data layer with Entity Framework Core configurations for all entities including the new fulfillment entities.

### AccountsPOC.WebAPI
REST API with new controllers:
- `PickListsController` - Pick list operations (CRUD, start, complete)
- `StockCountsController` - Stock counting operations (CRUD, reconcile)
- `DeliveryRoutesController` - Delivery route management with evidence capture and contact updates

### AccountsPOC.SharedComponents
Blazor Razor Class Library containing reusable components for both web and mobile:
- `StockCheckCard` - Display stock item information with quantity warnings
- `PickListCard` - Display pick list with items and status
- `DeliveryRouteCard` - Display delivery routes with stops
- `StockCountCard` - Display stock count with variances

### AccountsPOC.MauiApp
.NET MAUI Blazor Hybrid mobile application with the following features:

#### Features
1. **Stock Check** - Scan barcodes or manually enter stock codes to check inventory levels
2. **Pick Lists** - View and manage pick lists for order fulfillment
3. **Delivery Routes** - Manage delivery routes with:
   - GPS coordinates for each stop
   - Map integration (via CommunityToolkit.Maui.Maps)
   - Customer contact management
   - Delivery evidence capture (signatures and photos)
4. **Stock Counting** - Perform stock counts with variance tracking

#### Technology Stack
- .NET 10
- MAUI Blazor Hybrid (combines native MAUI with Blazor)
- ZXing.Net.Maui.Controls for barcode scanning
- CommunityToolkit.Maui for enhanced functionality
- CommunityToolkit.Maui.Maps for map features

#### Mobile-Specific Features
- Camera integration for delivery evidence
- Barcode scanner integration
- GPS/Location services for delivery routes
- Contact management for delivery customers
- Offline-capable architecture

## Building the Solution

### Prerequisites
- .NET 10 SDK
- For MAUI app development:
  - Visual Studio 2022 17.x or later with MAUI workload
  - Or VS Code with .NET MAUI extension
  - Android SDK (for Android)
  - Xcode (for iOS/Mac - macOS only)

### Build Commands

Build entire solution (except MAUI on Linux without workload):
```bash
dotnet build AccountsPOC.sln
```

Build individual projects:
```bash
dotnet build src/AccountsPOC.Domain/AccountsPOC.Domain.csproj
dotnet build src/AccountsPOC.Infrastructure/AccountsPOC.Infrastructure.csproj
dotnet build src/AccountsPOC.WebAPI/AccountsPOC.WebAPI.csproj
dotnet build src/AccountsPOC.BlazorApp/AccountsPOC.BlazorApp.csproj
dotnet build src/AccountsPOC.SharedComponents/AccountsPOC.SharedComponents.csproj
```

Build MAUI app (requires MAUI workload):
```bash
# Install MAUI workload first (Windows/Mac)
dotnet workload install maui

# Build for specific platforms
dotnet build src/AccountsPOC.MauiApp/AccountsPOC.MauiApp.csproj -f net10.0-android
dotnet build src/AccountsPOC.MauiApp/AccountsPOC.MauiApp.csproj -f net10.0-ios
```

## API Endpoints

### Pick Lists
- `GET /api/picklists` - Get all pick lists (with optional status filter)
- `GET /api/picklists/{id}` - Get specific pick list
- `POST /api/picklists` - Create new pick list
- `PUT /api/picklists/{id}` - Update pick list
- `PATCH /api/picklists/{id}/start` - Start picking
- `PATCH /api/picklists/{id}/complete` - Complete pick list
- `DELETE /api/picklists/{id}` - Delete pick list

### Stock Counts
- `GET /api/stockcounts` - Get all stock counts (with optional status filter)
- `GET /api/stockcounts/{id}` - Get specific stock count
- `POST /api/stockcounts` - Create new stock count
- `PUT /api/stockcounts/{id}` - Update stock count
- `PATCH /api/stockcounts/{id}/complete` - Complete count
- `PATCH /api/stockcounts/{id}/reconcile` - Reconcile count with inventory
- `DELETE /api/stockcounts/{id}` - Delete stock count

### Delivery Routes
- `GET /api/deliveryroutes` - Get all delivery routes (with optional filters)
- `GET /api/deliveryroutes/{id}` - Get specific route
- `POST /api/deliveryroutes` - Create new route
- `PUT /api/deliveryroutes/{id}` - Update route
- `PATCH /api/deliveryroutes/{id}/start` - Start route
- `PATCH /api/deliveryroutes/{id}/complete` - Complete route
- `PATCH /api/deliveryroutes/stops/{stopId}/update-contact` - Update stop contact info
- `PATCH /api/deliveryroutes/stops/{stopId}/capture-evidence` - Capture delivery evidence
- `DELETE /api/deliveryroutes/{id}` - Delete route

## Mobile App Usage

### Configuration
Update the API base URL in `MauiProgram.cs`:
```csharp
builder.Services.AddHttpClient("AccountsPOCAPI", client =>
{
    // For Android emulator: http://10.0.2.2:5000
    // For iOS simulator: http://localhost:5000
    // For production: https://your-api-url.com
    client.BaseAddress = new Uri("https://your-api-url.com/");
});
```

### Running the App
```bash
# Android
dotnet build -t:Run -f net10.0-android

# iOS (macOS only)
dotnet build -t:Run -f net10.0-ios

# Windows
dotnet build -t:Run -f net10.0-windows10.0.19041.0
```

## Database Migrations

After adding the new entities, create and apply migrations:

```bash
cd src/AccountsPOC.Infrastructure

# Create migration
dotnet ef migrations add AddFulfillmentEntities

# Apply migration
dotnet ef database update
```

## Architecture

The solution follows Clean Architecture principles:
- **Domain Layer** - Core business entities and logic
- **Application Layer** - Business logic and use cases
- **Infrastructure Layer** - Data access and external services
- **API Layer** - RESTful API endpoints
- **Presentation Layers** - Blazor Web App and MAUI Mobile App
- **Shared Components** - Reusable Blazor components

## Future Enhancements

Potential improvements:
1. Implement real-time barcode scanning in MAUI app
2. Add offline sync capabilities
3. Implement push notifications for route updates
4. Add real-time map tracking for delivery routes
5. Integrate with third-party shipping APIs
6. Add photo compression and cloud upload for delivery evidence
7. Implement voice-to-text for notes and comments
8. Add biometric authentication for mobile app

## License

[Your License Here]
