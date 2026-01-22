# Offline Sync Features

## Overview
The MAUI app includes comprehensive offline sync capabilities with visual status indicators and manual sync controls.

## Sync Status Bar

The app displays a fixed sync status bar at the top of the screen showing:

### Connection Status
- **📶 Online** - Device has internet connectivity (WiFi or Mobile Data)
- **📵 Offline** - Device is offline, data will be saved locally

### Sync Status Indicator
- **Spinning icon (⟳)** - Currently syncing data
- **Sync button (🔄)** - Click to manually trigger sync (enabled when online)
- **Pending badge** - Shows number of pending changes awaiting sync

### Last Sync Information
- Displays time since last successful sync
- Shows sync errors if they occur
- Auto-updates every 5 seconds

## Manual Sync

Users can trigger a manual sync by:
1. Tapping the sync button (🔄) in the top-right corner
2. Button is only enabled when device is online
3. Shows spinning animation during sync
4. Displays success/error message after completion

## Connectivity Detection

The app automatically detects:
- **WiFi connectivity** - Full speed syncing
- **Mobile data connectivity** - Syncing available
- **Offline mode** - Local-only operations

## Sync Process

1. **Download Phase**
   - Fetches latest delivery routes
   - Updates driver information
   - Syncs stock items
   - Downloads pick lists and stock counts

2. **Upload Phase**
   - Uploads pending delivery updates
   - Syncs barcode scan data
   - Updates stock count information
   - Sends evidence capture data

## Visual Indicators

### Status Colors
- **Green** - Successfully synced
- **Red** - Sync error occurred
- **Blue** - Currently syncing
- **Purple gradient** - Status bar background

### Animations
- **Pulse effect** - Online indicator
- **Spin animation** - Active sync
- **Slide down** - Status messages

## Testing Sync

To test the sync functionality:

1. **Online Mode**
   - Open the app with internet connection
   - Tap the sync button
   - Verify data is synced from server

2. **Offline Mode**
   - Turn off WiFi and mobile data
   - Make changes (update delivery stops, scan barcodes)
   - Changes are saved locally
   - Pending count increases

3. **Return Online**
   - Re-enable internet connection
   - Status changes to "Online"
   - Tap sync button or wait for auto-sync
   - Pending changes are uploaded

## Configuration

Sync settings in `MauiProgram.cs`:
```csharp
// SQLite database location
var dbPath = Path.Combine(FileSystem.AppDataDirectory, "accountspoc.db3");

// API endpoint configuration
client.BaseAddress = new Uri("https://localhost:5001/");
```

## Troubleshooting

**Sync Not Working?**
- Check internet connection
- Verify API endpoint is accessible
- Check for pending changes badge
- Look for error messages in status bar

**Pending Changes Not Syncing?**
- Ensure device is online
- Manually trigger sync
- Check API logs for errors
- Verify authentication if enabled

## Future Enhancements
- Conflict resolution UI
- Background sync scheduling
- Sync settings page
- Bandwidth optimization
- Selective sync options
