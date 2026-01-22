# 📱 AccountsPOC Mobile Fulfillment App - Complete Guide

> Amazon-style warehouse and delivery fulfillment mobile application built with .NET MAUI and Blazor Hybrid

## 📑 Table of Contents

1. [Overview](#overview)
2. [Features](#features)
3. [Screenshots](#screenshots)
4. [Installation](#installation)
5. [Configuration](#configuration)
6. [Usage Guide](#usage-guide)
7. [API Integration](#api-integration)
8. [Offline Mode](#offline-mode)
9. [Troubleshooting](#troubleshooting)

---

## 🎯 Overview

The AccountsPOC Mobile Fulfillment App is a comprehensive solution for warehouse operations and delivery management. Designed with Amazon fulfillment workflows in mind, it provides drivers and warehouse staff with powerful tools for managing their daily operations.

### Key Capabilities

- ✅ **Stock Management** - Check inventory, perform counts, track variances
- ✅ **Pick List Processing** - Fulfill orders with barcode scanning
- ✅ **Delivery Route Management** - Optimized routes with GPS navigation
- ✅ **Parcel Tracking** - Scan parcels into bags/cages for organized loading
- ✅ **Age Verification** - OTP-based verification for 18+ products
- ✅ **Offline Support** - Full SQLite database with automatic sync
- ✅ **Real-time Sync** - Visual status indicators and manual sync controls

---

## 🚀 Features

### 1. Home Dashboard
- Quick access to all key features
- Driver performance metrics
- Active route overview
- Sync status visibility

### 2. Stock Check
- **Barcode Scanning** - Use camera to scan product barcodes
- **Manual Entry** - Type stock codes manually
- **Real-time Information** - Current stock levels, bin locations, pricing
- **Low Stock Warnings** - Visual indicators for items below reorder level

### 3. Pick Lists
- **Active Pick Lists** - View all assigned pick lists
- **Item-by-Item Processing** - Track progress through each item
- **Bin Location Guidance** - Navigate to correct warehouse locations
- **Status Tracking** - Pending, In Progress, Completed states

### 4. Delivery Routes
- **Route Overview** - All stops with sequence numbers
- **GPS Navigation** - Lat/long coordinates for each stop
- **Customer Details** - Contact info, delivery instructions
- **Safe Places** - Porch, garage, custom locations
- **Access Codes** - Door codes, post box codes, building entry
- **Evidence Capture** - Take photos, collect signatures
- **Age Verification** - Generate and verify OTP for 18+ items

### 5. Stock Counting
- **Cycle Counts** - Regular stock verification
- **Variance Detection** - Automatic calculation of differences
- **Reconciliation** - Update system inventory after count
- **Item-by-Item Counting** - Scan or enter quantities for each item

### 6. Parcel Scanning
- **Container Types** - Bags (15 capacity), Cages (50 capacity), Loose items
- **Barcode Scanner** - Camera-based scanning with visual frame
- **Manual Entry** - Fallback for damaged/missing barcodes
- **Load Organization** - Track which parcels are in which containers
- **Capacity Tracking** - Real-time count per container

### 7. Route Organization
- **Drag-and-Drop** - Reorder stops with up/down buttons
- **Sequence Numbers** - Visual indicators for delivery order
- **Save to Server** - Persist optimized routes
- **Customer Preferences** - Consider time windows and instructions

### 8. Driver Dashboard
- **Performance Metrics** - Success rates, completion statistics
- **Active Route Progress** - Current stop, total stops
- **Route History** - Past 7 days performance
- **Vehicle Information** - Assigned vehicle details

### 9. Sync Status Bar
- **Connectivity Indicator** - WiFi/mobile data status
- **Sync Progress** - Spinning icon during active sync
- **Pending Changes** - Badge showing unsynced items
- **Manual Sync** - One-tap sync button
- **Last Sync Time** - Time since last successful sync

---

## 📸 Screenshots

> **Note**: Actual screenshots of the mobile application are available in the [`docs/screenshots/mobile/`](docs/screenshots/mobile/) directory. The ASCII art diagrams below provide a visual reference of the UI layout.

### Home Screen
```
┌─────────────────────────────────┐
│  📱 AccountsPOC Fulfillment     │
│  ┌───────────┐  ┌───────────┐  │
│  │ 📦 Stock  │  │ 📋 Pick   │  │
│  │   Check   │  │   Lists   │  │
│  └───────────┘  └───────────┘  │
│  ┌───────────┐  ┌───────────┐  │
│  │ 🚚 Routes │  │ 🔢 Stock  │  │
│  │           │  │  Counts   │  │
│  └───────────┘  └───────────┘  │
│  ┌───────────────────────────┐ │
│  │ 📦 Parcel Scanning        │ │
│  └───────────────────────────┘ │
│                                 │
│  🔄 Synced 2m ago  [📶 Online] │
└─────────────────────────────────┘
```

### Stock Check
```
┌─────────────────────────────────┐
│  📦 Stock Check                 │
│  ┌───────────────────────────┐ │
│  │ [📷] Scan Barcode         │ │
│  └───────────────────────────┘ │
│  ┌───────────────────────────┐ │
│  │ Enter Stock Code:         │ │
│  │ STK-001                   │ │
│  └───────────────────────────┘ │
│                                 │
│  ╔═══════════════════════════╗ │
│  ║ Standard Widget           ║ │
│  ║ Code: STK-001             ║ │
│  ║ Location: A1-B2-C3        ║ │
│  ║ On Hand: 100 units        ║ │
│  ║ Available: 90 units       ║ │
│  ║ Price: $19.99             ║ │
│  ╚═══════════════════════════╝ │
└─────────────────────────────────┘
```

### Delivery Route
```
┌─────────────────────────────────┐
│  🚚 Route ROUTE-001             │
│  Driver: John Driver            │
│  Vehicle: VAN-001               │
│                                 │
│  ┌─────────────────────────┐   │
│  │ ① Tech Solutions Inc    │   │
│  │   456 Business Ave      │   │
│  │   📞 +1234567894        │   │
│  │   ⏰ Scheduled          │   │
│  │   📍 View on Map        │   │
│  │   Safe: Porch           │   │
│  └─────────────────────────┘   │
│  ┌─────────────────────────┐   │
│  │ ② Retail Warehouse      │   │
│  │   789 Retail Blvd       │   │
│  │   📞 +1234567895        │   │
│  │   ⏰ Scheduled          │   │
│  │   🔐 Code: #1234        │   │
│  │   ⚠️ Age Verify Req     │   │
│  └─────────────────────────┘   │
└─────────────────────────────────┘
```

### Parcel Scanning
```
┌─────────────────────────────────┐
│  📦 Parcel Scanning             │
│  ┌─────────────────────────┐   │
│  │ Container Type:         │   │
│  │ [👜 Bag] [🗃️ Cage] [📦]  │   │
│  └─────────────────────────┘   │
│  ┌─────────────────────────┐   │
│  │ Select Container:       │   │
│  │ ▼ BAG-001 (5/15)        │   │
│  └─────────────────────────┘   │
│  ┌─────────────────────────┐   │
│  │ [📷 Scan Barcode]       │   │
│  └─────────────────────────┘   │
│                                 │
│  Scanned Parcels:               │
│  ✅ PKG-001 - 14:35            │
│  ✅ PKG-002 - 14:36            │
│  ✅ PKG-003 - 14:37            │
│  ✅ PKG-004 - 14:38            │
│  ✅ PKG-005 - 14:39            │
└─────────────────────────────────┘
```

### Driver Dashboard
```
┌─────────────────────────────────┐
│  📊 Driver Performance          │
│                                 │
│  ╔═══════════════════════════╗ │
│  ║ Total Routes: 45          ║ │
│  ║ Completed: 42             ║ │
│  ║ Success Rate: 93.3%       ║ │
│  ╚═══════════════════════════╝ │
│                                 │
│  Active Route:                  │
│  ┌─────────────────────────┐   │
│  │ ROUTE-001               │   │
│  │ Stop 1 of 2             │   │
│  │ [██████████░░░░░] 50%   │   │
│  └─────────────────────────┘   │
│                                 │
│  Vehicle: VAN-001               │
│  Status: On Route               │
│                                 │
│  [View Current Route]           │
└─────────────────────────────────┘
```

### Sync Status Bar
```
┌─────────────────────────────────┐
│  Top Right Corner:              │
│  ┌─────────────────────────┐   │
│  │ ⟳ Synced 5m ago        │   │
│  │ 📶 Online [🔄]  Badge:3│   │
│  └─────────────────────────┘   │
│                                 │
│  States:                        │
│  • 📶 Online (green pulse)     │
│  • 📵 Offline (red)            │
│  • ⟳ Syncing... (spinning)     │
│  • Badge (pending changes)      │
└─────────────────────────────────┘
```

---

## 💾 Installation

### Prerequisites

1. **.NET 10 SDK**
   ```bash
   # Verify installation
   dotnet --version
   # Should output: 10.x.x
   ```

2. **MAUI Workload**
   ```bash
   # Install MAUI workload
   dotnet workload install maui
   
   # Verify installation
   dotnet workload list
   ```

3. **Platform-Specific Requirements**

   **For Android:**
   - Android SDK (API 34 or higher)
   - Android Emulator or physical device
   - USB debugging enabled (for physical devices)

   **For iOS (macOS only):**
   - Xcode 15 or later
   - iOS 17 SDK
   - iOS Simulator or physical device
   - Apple Developer account (for device deployment)

   **For Windows:**
   - Windows 10/11 SDK (10.0.19041.0 or higher)
   - Visual Studio 2022 17.8+ recommended

### Build Instructions

1. **Clone the Repository**
   ```bash
   git clone https://github.com/dotnetappdev/accountspoc.git
   cd accountspoc
   ```

2. **Restore Dependencies**
   ```bash
   dotnet restore AccountsPOC.sln
   ```

3. **Build for Specific Platform**

   **Android:**
   ```bash
   cd src/AccountsPOC.MauiApp
   dotnet build -f net10.0-android
   ```

   **iOS:**
   ```bash
   dotnet build -f net10.0-ios
   ```

   **Windows:**
   ```bash
   dotnet build -f net10.0-windows10.0.19041.0
   ```

4. **Run the App**

   **Android:**
   ```bash
   dotnet build -t:Run -f net10.0-android
   ```

   **iOS:**
   ```bash
   dotnet build -t:Run -f net10.0-ios
   ```

---

## ⚙️ Configuration

### API Connection

Update the API base URL in `MauiProgram.cs`:

```csharp
// For Android Emulator
client.BaseAddress = new Uri("http://10.0.2.2:5000/");

// For iOS Simulator
client.BaseAddress = new Uri("http://localhost:5000/");

// For Physical Device (same network)
client.BaseAddress = new Uri("http://192.168.1.100:5000/");

// For Production
client.BaseAddress = new Uri("https://api.yourcompany.com/");
```

### Database Configuration

The app uses SQLite for offline storage. Database path is automatically configured:

```csharp
// Android: /data/data/com.accountspoc.mauiapp/files/accountspoc.db
// iOS: /var/mobile/Containers/Data/Application/[GUID]/Library/accountspoc.db
// Windows: C:\Users\[User]\AppData\Local\accountspoc\accountspoc.db
```

### Permissions

Required permissions are configured in platform-specific files:

**Android (`Platforms/Android/AndroidManifest.xml`):**
```xml
<uses-permission android:name="android.permission.INTERNET" />
<uses-permission android:name="android.permission.CAMERA" />
<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
<uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
```

**iOS (`Platforms/iOS/Info.plist`):**
```xml
<key>NSCameraUsageDescription</key>
<string>We need camera access to scan barcodes</string>
<key>NSLocationWhenInUseUsageDescription</key>
<string>We need location access for delivery routes</string>
```

---

## 📖 Usage Guide

### Getting Started

1. **Launch the App**
   - Open the app on your device
   - The app will automatically check for connectivity
   - Initial sync will download routes and inventory data

2. **Sync Your Data**
   - Tap the sync button (🔄) in the top-right
   - Wait for "Synced successfully" message
   - Green checkmark indicates data is up to date

### Daily Workflow for Drivers

#### Morning Routine

1. **Check In**
   - Open Driver Dashboard
   - Review assigned routes
   - Check vehicle assignment

2. **Scan Parcels to Van**
   - Navigate to "Parcel Scanning"
   - Select container type (Bag/Cage)
   - Scan each parcel barcode
   - Organize into containers
   - Verify all parcels loaded

3. **Review Route**
   - Open "Delivery Routes"
   - Review all stops in sequence
   - Note special instructions
   - Check for age-restricted items

#### During Deliveries

1. **Navigate to Stop**
   - Follow sequence numbers
   - Use GPS coordinates if needed
   - Note safe place instructions

2. **Standard Delivery**
   - Knock/ring doorbell
   - Hand package to customer
   - Capture signature/photo
   - Mark as "Delivered"

3. **Age-Restricted Delivery**
   - Verify customer age
   - Tap "Generate OTP"
   - Customer receives code via SMS/email
   - Enter OTP code to verify
   - Complete delivery only after verification

4. **Failed Delivery**
   - Select reason (Not home, refused, etc.)
   - Update safe place if available
   - Take photo of attempted delivery
   - Mark as "Failed"

#### End of Day

1. **Complete Route**
   - Mark route as "Complete"
   - Upload all photos/signatures
   - Sync data to server
   - Review performance metrics

### Offline Mode Usage

When internet connection is unavailable:

1. **Continue Working**
   - All features remain functional
   - Data stored locally in SQLite
   - Pending changes badge shows unsync count

2. **Sync When Online**
   - Status bar shows "📵 Offline"
   - When connection restored, shows "📶 Online"
   - Tap sync button or wait for auto-sync
   - Verify "Synced successfully" message

---

## 🔌 API Integration

### Base URL Configuration

The app communicates with the backend API for all data operations:

```
Base URL: https://your-api-url.com/api/
```

### Key Endpoints Used

**Authentication:**
- `POST /auth/login` - Driver login

**Delivery Routes:**
- `GET /deliveryroutes` - Get assigned routes
- `GET /deliveryroutes/{id}` - Get route details
- `PATCH /deliveryroutes/{id}/start` - Start route
- `PATCH /deliveryroutes/{id}/complete` - Complete route
- `POST /deliveryroutes/{id}/reorder-stops` - Save new stop order

**Delivery Stops:**
- `PATCH /deliveryroutes/stops/{stopId}/update-contact` - Update customer contact
- `PATCH /deliveryroutes/stops/{stopId}/capture-evidence` - Upload photo/signature
- `PATCH /deliveryroutes/stops/{stopId}/update-safe-place` - Update safe place
- `POST /deliveryroutes/stops/{stopId}/generate-otp` - Generate OTP
- `POST /deliveryroutes/stops/{stopId}/verify-otp` - Verify OTP

**Parcels:**
- `POST /parcels/scan-to-van` - Scan parcel to container
- `GET /parcels/by-barcode/{barcode}` - Lookup parcel
- `GET /containers/by-route/{routeId}` - Get route containers

**Stock:**
- `GET /stockitems/by-code/{code}` - Get stock item
- `GET /stockitems/by-barcode/{barcode}` - Get by barcode

**Pick Lists:**
- `GET /picklists` - Get all pick lists
- `GET /picklists/{id}` - Get pick list details
- `PATCH /picklists/{id}/start` - Start picking
- `PATCH /picklists/{id}/complete` - Complete pick list

**Stock Counts:**
- `GET /stockcounts` - Get all stock counts
- `POST /stockcounts` - Create new count
- `PATCH /stockcounts/{id}/complete` - Complete count
- `PATCH /stockcounts/{id}/reconcile` - Reconcile count

**Dashboard:**
- `GET /dashboard/driver-performance/{driverId}` - Get driver stats

---

## 📴 Offline Mode

### How It Works

The app uses a hybrid online/offline architecture:

1. **Local SQLite Database**
   - Mirrors server data structure
   - Stores all entities locally
   - Fast access without network

2. **Sync Service**
   - Bi-directional synchronization
   - Download: Server → Local
   - Upload: Local → Server
   - Conflict resolution

3. **Change Tracking**
   - `SyncLog` table tracks all changes
   - Timestamps for created/updated/deleted
   - Pending changes badge in UI

### Sync Triggers

**Automatic Sync:**
- App startup (if online)
- Every 5 minutes (background)
- After completing route
- When returning online

**Manual Sync:**
- Tap sync button (🔄)
- Pull to refresh (on list screens)

### Synced Data

✅ Delivery routes and stops  
✅ Driver information  
✅ Customer details  
✅ Stock items and inventory  
✅ Pick lists  
✅ Stock counts  
✅ Parcels and containers  
✅ Warehouses  

### Data Not Synced Immediately

⏳ Photos (uploaded when online)  
⏳ Signatures (uploaded when online)  
⏳ Large attachments

### Troubleshooting Sync Issues

**Sync Failed Error:**
1. Check internet connection
2. Verify API URL is correct
3. Check device time/date settings
4. Review pending changes (may have conflicts)

**Pending Changes Not Uploading:**
1. Manually tap sync button
2. Check for server errors
3. Verify authentication token
4. Clear app data and re-sync (last resort)

---

## 🔧 Troubleshooting

### Common Issues

#### 1. Camera Not Working

**Symptom:** Barcode scanner shows black screen

**Solutions:**
- Check camera permissions in device settings
- Restart the app
- On Android: Settings → Apps → AccountsPOC → Permissions → Camera → Allow
- On iOS: Settings → Privacy → Camera → AccountsPOC → Enable

#### 2. GPS/Location Not Available

**Symptom:** "Location unavailable" on delivery routes

**Solutions:**
- Enable location services in device settings
- Grant location permission to app
- Ensure GPS is enabled
- Try moving to open area (away from buildings)

#### 3. Sync Errors

**Symptom:** "Sync failed" messages

**Solutions:**
- Check internet connection (WiFi/mobile data)
- Verify API URL is correct
- Restart app
- Clear cache and re-sync

#### 4. Slow Performance

**Symptom:** App is laggy or unresponsive

**Solutions:**
- Clear local database: Settings → Clear Data
- Restart device
- Check available storage space
- Update to latest app version

#### 5. Login Issues

**Symptom:** Cannot log in to app

**Solutions:**
- Verify credentials are correct
- Check API connectivity
- Ensure driver account is active
- Contact administrator for account verification

### Debug Mode

To enable debug logging:

1. Open app settings
2. Tap "About" 7 times
3. Developer mode enabled
4. View logs in Settings → Debug Logs

### Getting Help

**Support Channels:**
- Email: support@accountspoc.com
- Phone: +1-800-ACCOUNTS
- Help Desk: https://support.accountspoc.com
- Documentation: https://docs.accountspoc.com

**When Reporting Issues:**
- Include app version
- Describe steps to reproduce
- Include screenshots if possible
- Check debug logs for errors

---

## 📝 Notes

### Security

- All API communication uses HTTPS
- Authentication tokens expire after 8 hours
- Offline data encrypted at rest
- Photos/signatures stored securely

### Performance

- Optimized for low-bandwidth scenarios
- Efficient battery usage
- Minimal data usage (typically < 50MB/day)
- Works on devices with 2GB+ RAM

### Compliance

- GDPR compliant data handling
- Customer data encrypted
- Audit trail for all deliveries
- Age verification tracking

---

## 🎓 Training Resources

### Video Tutorials

1. **Getting Started** (5 min) - App overview and setup
2. **Daily Delivery Workflow** (10 min) - Complete walkthrough
3. **Parcel Scanning** (5 min) - Container management
4. **Offline Mode** (5 min) - Working without connectivity
5. **Route Organization** (5 min) - Optimizing delivery order

### Quick Reference Cards

- **Keyboard Shortcuts** - Speed up data entry
- **Barcode Formats** - Supported barcode types
- **Status Codes** - Delivery status meanings
- **Error Messages** - Common errors and solutions

---

## 📊 Performance Metrics

Track your performance with built-in analytics:

- **Success Rate** - Percentage of successful deliveries
- **Average Delivery Time** - Time per stop
- **Routes Completed** - Total completed routes
- **Parcels Scanned** - Total parcels handled
- **Customer Satisfaction** - Based on feedback

---

## 🔄 Updates

The app receives regular updates with new features and improvements:

- **Auto-update** - Enabled by default
- **Release Notes** - View in app settings
- **Beta Program** - Test new features early
- **Feedback** - Submit feature requests

---

## 📞 Contact

For questions or support:

**Technical Support:**  
Email: tech@accountspoc.com  
Phone: +1-800-TECH-SUP

**Sales Inquiries:**  
Email: sales@accountspoc.com  
Phone: +1-800-SALES-01

**Documentation:**  
https://docs.accountspoc.com/mobile-app

---

## 📜 License

Copyright © 2026 AccountsPOC. All rights reserved.

This software is proprietary and confidential. Unauthorized copying, distribution, or use is strictly prohibited.

---

**Version:** 1.0.0  
**Last Updated:** January 2026  
**Platform:** .NET MAUI 10.0
