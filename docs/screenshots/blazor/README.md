# Blazor Web Application Screenshots

This directory contains screenshots of the AccountsPOC Blazor Web Application.

## Screenshots

The following screenshots document the key features of the Blazor web interface:

### Navigation and Dashboard
- `01-home-dashboard.png` - Main landing page with navigation and overview
- `09-management-dashboard.png` - Analytics and reporting dashboard

### Core Accounting Features
- `02-bank-accounts.png` - Bank account management
- `03-stock-items.png` - Product/stock item catalog
- `04-sales-orders.png` - Sales order processing
- `05-sales-invoices.png` - Invoice management
- `06-bill-of-materials.png` - Bill of Materials (BOM) management

### Additional Features
- `07-price-lists.png` - Price list management
- `08-invoice-payment.png` - Payment processing
- `10-settings.png` - Application settings

## How to Capture

1. Start the Web API:
   ```bash
   cd src/AccountsPOC.WebAPI
   dotnet run
   ```

2. Start the Blazor App:
   ```bash
   cd src/AccountsPOC.BlazorApp
   dotnet run
   ```

3. Navigate to `http://localhost:5193` in your browser

4. Capture screenshots of each page as you navigate through the application

## Screenshot Guidelines

- Use 1920x1080 resolution or full browser window
- Capture in PNG format for best quality
- Include realistic sample data
- Show complete interface with navigation
- Ensure clean, production-ready state

For detailed guidelines, see [SCREENSHOTS_GUIDE.md](../../SCREENSHOTS_GUIDE.md).
