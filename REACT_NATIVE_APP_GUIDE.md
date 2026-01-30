# React Native Contractor App

## Overview

A cross-platform mobile application built with React Native and Expo that allows contractors to manage sales orders, quotes, and work orders. The app features offline-first functionality with SQLite storage and can sync with the main AccountsPOC backend API.

## Location

The React Native app is located in the `ContractorApp/` directory at the root of the repository.

## Key Features

- **Sales Orders Management** - Create, view, edit, and delete sales orders with line items
- **Quotes Management** - Manage customer quotes with ability to convert to sales orders
- **Work Orders Management** - Track work orders with tasks and site visit sign-offs
- **Offline-First Architecture** - All data stored locally in SQLite for offline access
- **Bidirectional Sync** - Push and pull data from the AccountsPOC Web API
- **Settings Management** - Configure API endpoints and manage sync preferences
- **Test Data Seeding** - Optionally populate the database with sample data
- **Cross-Platform Support** - Runs on iOS, Android, Web, Windows, and Linux

## Technology Stack

- **React Native with Expo** - Cross-platform mobile development
- **TypeScript** - Type-safe development
- **expo-sqlite** - Local SQLite database
- **React Navigation** - Navigation and routing
- **Axios** - HTTP client for API communication
- **React Native Vector Icons** - UI iconography

## Getting Started

### Prerequisites

- Node.js v16 or higher
- npm or yarn
- Expo CLI (install with `npm install -g expo-cli`)

For platform-specific development:
- **iOS**: macOS with Xcode
- **Android**: Android Studio with Android SDK

### Installation

1. Navigate to the ContractorApp directory:
   ```bash
   cd ContractorApp
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

3. Start the development server:
   ```bash
   npm start
   ```

### Running the App

- **Web**: `npm run web`
- **iOS**: `npm run ios` (macOS only)
- **Android**: `npm run android`
- **Physical Device**: Use Expo Go app and scan QR code

## Database Schema

The app uses SQLite with the following main tables:

- **sales_orders** - Sales order headers with customer information
- **sales_order_items** - Line items for sales orders
- **quotes** - Quote headers with customer and expiry information
- **quote_items** - Line items for quotes
- **work_orders** - Work order headers with site and scheduling details
- **work_order_tasks** - Individual tasks within work orders
- **site_visit_signoffs** - Sign-off records for site visits
- **settings** - App configuration and sync settings

## API Integration

The app integrates with the AccountsPOC Web API:

### Endpoints Used

- `/api/SalesOrders` - CRUD operations for sales orders
- `/api/Quotes` - CRUD operations for quotes
- `/api/Quotes/{id}/convert-to-order` - Convert quote to sales order
- `/api/WorkOrders` - CRUD operations for work orders
- `/api/SiteVisitSignOffs` - CRUD operations for site visit sign-offs

### Configuration

Default API URL: `http://localhost:5001/api`

To configure:
1. Open the app
2. Navigate to Settings tab
3. Update the API URL
4. Enable/disable synchronization

### Sync Process

1. **Sync to Server**: Pushes all records marked as 'pending' to the API
2. **Sync from Server**: Pulls data from the API and stores locally
3. Tracks sync status per record (pending, synced)
4. Records last sync timestamp

## App Structure

```
ContractorApp/
├── src/
│   ├── components/       # Reusable UI components (future)
│   ├── database/        # SQLite database setup and seed data
│   │   └── database.ts
│   ├── navigation/      # Navigation configuration
│   │   └── Navigation.tsx
│   ├── screens/         # App screens
│   │   ├── HomeScreen.tsx
│   │   ├── SalesOrdersListScreen.tsx
│   │   ├── SalesOrderFormScreen.tsx
│   │   ├── QuotesListScreen.tsx
│   │   ├── QuoteFormScreen.tsx
│   │   ├── WorkOrdersListScreen.tsx
│   │   ├── WorkOrderFormScreen.tsx
│   │   └── SettingsScreen.tsx
│   ├── services/        # API and business logic
│   │   └── apiService.ts
│   ├── types/           # TypeScript type definitions
│   │   └── index.ts
│   └── utils/           # Utility functions (future)
├── assets/              # Images and icons
├── App.tsx             # App entry point
├── package.json        # Dependencies
└── README.md          # App-specific documentation
```

## Usage Guide

### Managing Sales Orders

1. **View Orders**: Tap the "Sales Orders" tab to see all orders
2. **Create Order**: Tap the "+" floating button, fill in details, and save
3. **Edit Order**: Tap "Edit" on any order card
4. **Delete Order**: Tap "Delete" on any order card

### Settings and Configuration

1. **API Configuration**: Set the backend API URL
2. **Sync Management**: 
   - Toggle sync on/off
   - Manually sync to/from server
   - View last sync time
3. **Test Data**: Seed sample data for testing
4. **Data Management**: Clear all local data if needed

### Testing with Seed Data

1. Go to Settings tab
2. Tap "Seed Test Data"
3. Confirm the action
4. Sample records will be created:
   - 1 Sales Order with 2 items
   - 1 Quote with 1 item
   - 1 Work Order with 3 tasks

## Development Notes

### Current Implementation Status

✅ **Completed:**
- Full Sales Orders CRUD functionality
- Settings screen with sync and seed options
- Database schema and initialization
- API service layer with sync capabilities
- Navigation structure
- Home dashboard with statistics

🚧 **Partial Implementation:**
- Quotes screens (placeholder UI)
- Work Orders screens (placeholder UI)

📝 **Future Enhancements:**
- Complete Quotes and Work Orders screens
- Photo capture for site visits
- Signature capture for sign-offs
- Offline queue management
- Conflict resolution
- Push notifications
- Barcode scanning
- PDF generation

### Adding New Features

1. **New Screen**: Create in `src/screens/` and add to navigation
2. **New API Endpoint**: Add method to `apiService.ts`
3. **New Database Table**: Update `database.ts` with schema

## Backend Integration

This app is designed to work with the AccountsPOC backend system:

### Backend Requirements

The backend must be running and accessible:
1. Start the AccountsPOC.WebAPI project
2. Ensure it's listening on `http://localhost:5001`
3. Verify API endpoints are accessible

### New Backend Features

This implementation added the following to the backend:
- Quote entity and QuoteItem entity
- WorkOrder entity and WorkOrderTask entity
- SiteVisitSignOff entity
- API controllers for all new entities
- Database migrations
- Blazor pages for managing quotes, work orders, and sign-offs

## Troubleshooting

### Common Issues

**Database errors**: Clear app data and restart
**API connection issues**: 
- Verify API URL in Settings
- Check backend is running
- For Android emulator, use `http://10.0.2.2:5001/api`
**Build issues**: Clear cache with `expo start -c`

## Platform-Specific Notes

### iOS
- Requires macOS for development
- Use `http://localhost:5001/api` for simulator
- Physical device testing requires Apple Developer account

### Android
- Can develop on any platform
- Use `http://10.0.2.2:5001/api` for emulator
- Enable developer mode on physical device

### Web
- Runs in any modern browser
- Use `http://localhost:5001/api`
- Some mobile features may not work

### Windows/Linux
- Run as web app via `npm run web`
- Full functionality available through browser

## Security Considerations

- API communication is not encrypted in development
- No authentication implemented yet
- Database is not encrypted
- For production, implement:
  - HTTPS for API
  - User authentication
  - Database encryption
  - Secure token storage

## Contributing

When contributing to this app:
1. Follow the existing code structure
2. Add TypeScript types for new features
3. Test on multiple platforms
4. Update documentation
5. Ensure database migrations are reversible

## License

This is a proof-of-concept application for demonstration purposes.

## Related Documentation

- Main project README: `/README.md`
- Blazor App Guide: `/BLAZOR_APP_GUIDE.md`
- API documentation: See WebAPI controllers
- Database setup: `/DATABASE_SETUP_GUIDE.md`

## Support

For issues and questions:
1. Check the README.md in ContractorApp/
2. Review the main repository documentation
3. Check console logs for errors
4. Verify API connectivity
