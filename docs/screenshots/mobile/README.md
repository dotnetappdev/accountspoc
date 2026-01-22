# MAUI Mobile Application Screenshots

This directory contains screenshots of the AccountsPOC MAUI Mobile Fulfillment Application.

## Screenshots

The following screenshots document the key features of the mobile application:

### Home and Navigation
- `01-home-dashboard.png` - Main driver dashboard with quick access tiles

### Stock Management
- `02-stock-check.png` - Stock check interface
- `03-stock-check-scanner.png` - Barcode scanner camera view
- `04-stock-check-results.png` - Stock item details and information
- `11-stock-counts.png` - Stock counting operations

### Order Fulfillment
- `05-pick-lists.png` - Active pick lists overview
- `06-pick-list-detail.png` - Individual pick list with items

### Delivery Operations
- `07-delivery-routes.png` - Delivery routes list
- `08-delivery-route-detail.png` - Route details with stops and map
- `09-delivery-stop-detail.png` - Individual stop with customer info
- `13-route-organizer.png` - Route planning and organization

### Parcel Management
- `10-parcel-scanning.png` - Parcel scanning and bag/cage assignment

### Sync and Status
- `12-sync-status.png` - Sync status indicators and controls

## How to Capture

1. Install MAUI workload:
   ```bash
   dotnet workload install maui
   ```

2. Build and run the mobile app:
   ```bash
   cd src/AccountsPOC.MauiApp
   
   # For Android
   dotnet build -f net10.0-android
   dotnet run -f net10.0-android
   
   # For iOS
   dotnet build -f net10.0-ios
   dotnet run -f net10.0-ios
   ```

3. Use device screen capture tools to take screenshots:
   - **Android**: Power + Volume Down
   - **iOS**: Side Button + Volume Up

## Screenshot Guidelines

- Use native device resolution
- Capture in PNG format
- Show realistic warehouse/delivery scenarios
- Include all mobile-specific features (camera, GPS, etc.)
- Demonstrate both online and offline modes
- Show sync status clearly

For detailed guidelines, see [SCREENSHOTS_GUIDE.md](../../SCREENSHOTS_GUIDE.md).
