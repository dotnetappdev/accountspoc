# Screenshots Guide - AccountsPOC

This guide provides instructions for capturing and documenting screenshots for both the Blazor Web Application and the MAUI Mobile Application.

## Overview

Screenshots should be captured for all major features and user interfaces to help users understand the application's capabilities and user experience.

## Directory Structure

```
docs/
├── screenshots/
│   ├── blazor/          # Blazor web app screenshots
│   └── mobile/          # MAUI mobile app screenshots
└── SCREENSHOTS_GUIDE.md # This file
```

## Blazor Web Application Screenshots

### Required Screenshots

The following screenshots should be captured from the Blazor web application running at `http://localhost:5193`:

#### 1. Home Dashboard (`blazor/01-home-dashboard.png`)
- **Description**: Main landing page showing overview and navigation
- **Key Elements**: Navigation menu, dashboard widgets, quick access buttons
- **How to Capture**: Navigate to home page after starting the application

#### 2. Bank Accounts (`blazor/02-bank-accounts.png`)
- **Description**: Bank accounts management screen
- **Key Elements**: List of bank accounts, balance display, add/edit/delete buttons
- **How to Capture**: Navigate to Bank Accounts menu item

#### 3. Products/Stock Items (`blazor/03-stock-items.png`)
- **Description**: Product catalog management
- **Key Elements**: Product list, stock levels, pricing, reorder levels
- **How to Capture**: Navigate to Products/Stock Items menu item

#### 4. Sales Orders (`blazor/04-sales-orders.png`)
- **Description**: Sales order management
- **Key Elements**: Order list, customer details, order items, status
- **How to Capture**: Navigate to Sales Orders menu item

#### 5. Sales Invoices (`blazor/05-sales-invoices.png`)
- **Description**: Invoice management and generation
- **Key Elements**: Invoice list, amounts, dates, payment status
- **How to Capture**: Navigate to Sales Invoices menu item

#### 6. Bill of Materials (`blazor/06-bill-of-materials.png`)
- **Description**: BOM management screen
- **Key Elements**: BOM list, components, quantities
- **How to Capture**: Navigate to Bill of Materials menu item

#### 7. Price Lists (`blazor/07-price-lists.png`)
- **Description**: Price list management
- **Key Elements**: Price lists, customer-specific pricing
- **How to Capture**: Navigate to Price Lists menu item

#### 8. Invoice Payment (`blazor/08-invoice-payment.png`)
- **Description**: Payment processing screen
- **Key Elements**: Payment form, invoice selection, amount input
- **How to Capture**: Navigate to Invoice Payment menu item

#### 9. Management Dashboard (`blazor/09-management-dashboard.png`)
- **Description**: Analytics and reporting dashboard
- **Key Elements**: Charts, metrics, KPIs
- **How to Capture**: Navigate to Management Dashboard menu item

#### 10. Settings (`blazor/10-settings.png`)
- **Description**: Application configuration
- **Key Elements**: Settings forms, configuration options
- **How to Capture**: Navigate to Settings menu item

### How to Run Blazor App for Screenshots

```bash
# Terminal 1: Start the Web API
cd src/AccountsPOC.WebAPI
dotnet run

# Terminal 2: Start the Blazor App
cd src/AccountsPOC.BlazorApp
dotnet run

# Access at http://localhost:5193
```

## MAUI Mobile Application Screenshots

### Required Screenshots

The following screenshots should be captured from the MAUI mobile application:

#### 1. Home/Driver Dashboard (`mobile/01-home-dashboard.png`)
- **Description**: Main mobile home screen
- **Key Elements**: Quick access tiles, sync status, driver metrics
- **How to Capture**: Launch app to see home screen

#### 2. Stock Check (`mobile/02-stock-check.png`)
- **Description**: Stock checking interface
- **Key Elements**: Barcode scanner button, stock search, item details
- **How to Capture**: Navigate to Stock Check from home

#### 3. Stock Check - Scanner (`mobile/03-stock-check-scanner.png`)
- **Description**: Barcode scanning interface
- **Key Elements**: Camera view, scanner overlay, cancel button
- **How to Capture**: Click "Scan Barcode" button in Stock Check

#### 4. Stock Check - Results (`mobile/04-stock-check-results.png`)
- **Description**: Stock item information display
- **Key Elements**: Product details, stock levels, location, price
- **How to Capture**: After scanning or searching for a product

#### 5. Pick Lists (`mobile/05-pick-lists.png`)
- **Description**: Active pick lists overview
- **Key Elements**: List of pick lists, status, item counts
- **How to Capture**: Navigate to Pick Lists from home

#### 6. Pick List Detail (`mobile/06-pick-list-detail.png`)
- **Description**: Individual pick list with items
- **Key Elements**: Items to pick, quantities, locations, completion status
- **How to Capture**: Tap on a pick list

#### 7. Delivery Routes (`mobile/07-delivery-routes.png`)
- **Description**: Delivery routes list
- **Key Elements**: Route list, stop counts, addresses
- **How to Capture**: Navigate to Delivery Routes from home

#### 8. Delivery Route Detail (`mobile/08-delivery-route-detail.png`)
- **Description**: Individual route with stops
- **Key Elements**: Stop list, addresses, map view, navigation buttons
- **How to Capture**: Tap on a delivery route

#### 9. Delivery Stop Detail (`mobile/09-delivery-stop-detail.png`)
- **Description**: Individual delivery stop
- **Key Elements**: Customer details, contact info, evidence capture buttons
- **How to Capture**: Tap on a delivery stop

#### 10. Parcel Scanning (`mobile/10-parcel-scanning.png`)
- **Description**: Parcel/package scanning interface
- **Key Elements**: Scanner, bag/cage assignment, parcel list
- **How to Capture**: Navigate to Parcel Scanning from home

#### 11. Stock Counts (`mobile/11-stock-counts.png`)
- **Description**: Stock counting interface
- **Key Elements**: Count list, variance indicators
- **How to Capture**: Navigate to Stock Counts from home

#### 12. Sync Status (`mobile/12-sync-status.png`)
- **Description**: Sync status bar and indicators
- **Key Elements**: Sync status icon, last sync time, manual sync button
- **How to Capture**: Top of any screen showing sync status bar

#### 13. Route Organizer (`mobile/13-route-organizer.png`)
- **Description**: Route organization and planning
- **Key Elements**: Route planning interface, optimization options
- **How to Capture**: Navigate to Route Organizer from home

### How to Run MAUI App for Screenshots

```bash
# Install MAUI workload (if not already installed)
dotnet workload install maui

# Build and run on Android
cd src/AccountsPOC.MauiApp
dotnet build -f net10.0-android
dotnet run -f net10.0-android

# OR run on iOS
dotnet build -f net10.0-ios
dotnet run -f net10.0-ios
```

## Screenshot Standards

### Technical Requirements
- **Format**: PNG (preferred) or JPEG
- **Resolution**: 
  - Blazor: 1920x1080 or actual browser window size
  - Mobile: Native device resolution (e.g., 1080x2340 for Android)
- **File Naming**: Use descriptive names with numbers (e.g., `01-home-dashboard.png`)

### Capture Guidelines
1. **Clean Data**: Use realistic but clean sample data
2. **Full Screen**: Capture the complete interface
3. **Hide Sensitive Info**: Blur or remove any sensitive information
4. **Good Lighting**: Ensure mobile screenshots are well-lit and clear
5. **Consistent State**: Show the app in a consistent, ready state

### What to Show
- ✅ Main navigation and menu structures
- ✅ Key data entry forms
- ✅ List views with sample data
- ✅ Detail views showing complete information
- ✅ Important action buttons and controls
- ✅ Mobile-specific features (scanner, camera, GPS)

### What to Avoid
- ❌ Error states or broken UI
- ❌ Empty screens (add sample data first)
- ❌ Personal or sensitive information
- ❌ Development/debug information
- ❌ Incomplete or loading states

## Using Screenshots in Documentation

Once captured, reference screenshots in documentation using relative paths:

```markdown
### Home Dashboard
![Home Dashboard](screenshots/blazor/01-home-dashboard.png)

The home dashboard provides quick access to all major features...
```

## Updating Screenshots

When updating screenshots:
1. Capture all affected views
2. Maintain the same naming convention
3. Update file modification date
4. Update related documentation references
5. Ensure consistency across the screenshot set

## Contributing Screenshots

If you're contributing screenshots:
1. Follow the naming conventions in this guide
2. Ensure high quality and clarity
3. Place files in the correct directories
4. Update this guide if adding new sections
5. Submit with clear descriptions of what each shows

---

**Note**: This is a living document. Update it as new features are added or UI changes occur.
