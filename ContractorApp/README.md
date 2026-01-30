# Contractor App - React Native Mobile Application

A cross-platform mobile application built with React Native and Expo for contractors to manage sales orders, quotes, and work orders.

## Features

### Core Functionality
- **Sales Orders Management** - Create, view, edit, and delete sales orders
- **Quotes Management** - Manage customer quotes with conversion to orders
- **Work Orders Management** - Track work orders with tasks and site visit sign-offs
- **Offline-First** - All data stored locally in SQLite database
- **API Synchronization** - Sync data with AccountsPOC backend API
- **Settings Management** - Configure API endpoints and sync options
- **Test Data Seeding** - Optionally seed sample data for testing

### Platform Support
- ✅ iOS
- ✅ Android
- ✅ Web
- ✅ Windows (via web)
- ✅ Linux (via web)

## Technology Stack

- **React Native** - Cross-platform mobile framework
- **Expo** - Development and build tooling
- **TypeScript** - Type-safe development
- **expo-sqlite** - Local database storage
- **React Navigation** - Navigation between screens
- **Axios** - API communication
- **React Native Vector Icons** - UI icons

## Prerequisites

Before you begin, ensure you have the following installed:
- Node.js (v16 or higher)
- npm or yarn
- Expo CLI: `npm install -g expo-cli`

For iOS development:
- macOS
- Xcode

For Android development:
- Android Studio
- Android SDK

## Installation

1. **Navigate to the app directory:**
   ```bash
   cd ContractorApp
   ```

2. **Install dependencies:**
   ```bash
   npm install
   ```

3. **Start the development server:**
   ```bash
   npm start
   ```

## Running the App

### Web
```bash
npm run web
```
The app will open in your default browser at `http://localhost:8081`

### iOS Simulator (macOS only)
```bash
npm run ios
```

### Android Emulator
```bash
npm run android
```

### Physical Device
1. Install the Expo Go app on your device
2. Scan the QR code from the terminal

## Database Structure

The app uses SQLite with the following tables:

### Settings
- Stores API configuration and sync settings

### Sales Orders
- id, orderNumber, customerName, customerEmail, customerPhone
- orderDate, totalAmount, status, notes
- syncStatus, createdAt, updatedAt

### Quotes
- id, quoteNumber, customerName, customerEmail, customerPhone
- quoteDate, expiryDate, totalAmount, status, notes
- syncStatus, createdAt, updatedAt

### Work Orders
- id, workOrderNumber, customerName, description
- workOrderDate, scheduledDate, completedDate
- status, priority, siteAddress, estimatedHours, actualHours
- syncStatus, createdAt, updatedAt

### Supporting Tables
- sales_order_items
- quote_items
- work_order_tasks
- site_visit_signoffs

## API Integration

The app integrates with the AccountsPOC Web API:

### Default Configuration
- Base URL: `http://localhost:5001/api`
- Can be configured in Settings screen

### API Endpoints Used
- `/SalesOrders` - CRUD operations for sales orders
- `/Quotes` - CRUD operations for quotes
- `/WorkOrders` - CRUD operations for work orders
- `/SiteVisitSignOffs` - CRUD operations for sign-offs

### Sync Functionality
- **Sync to Server** - Pushes local pending changes to the API
- **Sync from Server** - Pulls data from the API to local database
- Tracks sync status per record
- Stores last sync timestamp

## Configuration

### Settings Screen
Access via the Settings tab in the app:

1. **API Configuration**
   - Set the API URL
   - Enable/disable synchronization

2. **Data Management**
   - Seed test data
   - Sync to/from server
   - Clear all local data

3. **Sync Status**
   - View last sync timestamp
   - Monitor pending sync items

## Usage

### Creating a Sales Order

1. Navigate to the "Sales Orders" tab
2. Tap the "+" button (floating action button)
3. Fill in the order details:
   - Order Number (auto-generated)
   - Customer Name (required)
   - Customer Email
   - Customer Phone
   - Total Amount
   - Status
   - Notes
4. Tap "Save Order"

### Syncing with Server

1. Go to the Settings tab
2. Ensure "Enable Sync" is turned on
3. Configure the API URL if needed
4. Tap "Sync to Server" to push local changes
5. Tap "Sync from Server" to pull remote data

### Seeding Test Data

1. Go to the Settings tab
2. Tap "Seed Test Data"
3. Confirm the action
4. Sample sales orders, quotes, and work orders will be created

## Development

### Project Structure
```
ContractorApp/
├── src/
│   ├── components/       # Reusable UI components
│   ├── database/        # SQLite database setup
│   ├── navigation/      # Navigation configuration
│   ├── screens/         # App screens
│   ├── services/        # API and business logic
│   ├── types/           # TypeScript type definitions
│   └── utils/           # Utility functions
├── App.tsx              # App entry point
├── package.json         # Dependencies
└── README.md           # This file
```

### Adding New Screens

1. Create screen component in `src/screens/`
2. Add screen to navigation in `src/navigation/Navigation.tsx`
3. Update types in navigation types

### Database Migrations

To modify the database schema:
1. Update `src/database/database.ts`
2. Modify the `initDatabase()` function
3. Users may need to clear app data

## Troubleshooting

### Database Initialization Issues
- Clear app data and restart
- Check console for error messages

### API Connection Issues
- Verify API URL in Settings
- Ensure the backend API is running
- Check network connectivity
- For iOS simulator, use `http://localhost:5001/api`
- For Android emulator, use `http://10.0.2.2:5001/api`

### Build Issues
- Clear node_modules: `rm -rf node_modules && npm install`
- Clear Expo cache: `expo start -c`
- Rebuild: `expo start --clear`

## Building for Production

### iOS
```bash
expo build:ios
```

### Android
```bash
expo build:android
```

### Web
```bash
npm run build:web
```

## Future Enhancements

- [ ] Complete implementation of Quotes screens
- [ ] Complete implementation of Work Orders screens
- [ ] Add photo capture for site visits
- [ ] Implement signature capture
- [ ] Add offline detection and queue management
- [ ] Implement conflict resolution for sync
- [ ] Add push notifications
- [ ] Implement barcode scanning
- [ ] Add PDF generation
- [ ] Implement advanced filtering and search

## License

This is a proof-of-concept application for demonstration purposes.

## Support

For issues and questions, please refer to the main AccountsPOC repository.
