# 💻 AccountsPOC Blazor Web Application - Complete Guide

> Modern, full-featured accounting management system built with Blazor Web App and ASP.NET Core

## 📑 Table of Contents

1. [Overview](#overview)
2. [Features](#features)
3. [Screenshots](#screenshots)
4. [Installation](#installation)
5. [Configuration](#configuration)
6. [Usage Guide](#usage-guide)
7. [API Integration](#api-integration)
8. [Architecture](#architecture)
9. [Troubleshooting](#troubleshooting)

---

## 🎯 Overview

The AccountsPOC Blazor Web Application is a comprehensive accounting solution similar to Sage 200, providing full-featured financial and inventory management capabilities through a modern, interactive web interface.

### Key Capabilities

- ✅ **Multi-Bank Account Management** - Track multiple bank accounts with balances
- ✅ **Product Catalog** - Complete inventory management with stock control
- ✅ **Sales Order Processing** - Customer order management with line items
- ✅ **Invoice Generation** - Automated invoice creation from sales orders
- ✅ **Bill of Materials** - BOM management with component tracking
- ✅ **Price Lists** - Customer-specific pricing strategies
- ✅ **Payment Processing** - Invoice payment tracking and management
- ✅ **Management Reporting** - Analytics dashboard with KPIs

---

## 🚀 Features

### 1. Home Dashboard
- Quick access to all features
- Navigation sidebar with modern UI
- Theme toggle (light/dark mode)
- Responsive layout for all devices

### 2. Bank Accounts Management
- **Multiple Accounts** - Track unlimited bank accounts
- **Balance Monitoring** - Real-time balance tracking
- **Account Details** - Account numbers, sort codes, IBAN
- **Transaction History** - Full audit trail

### 3. Product/Stock Management
- **Product Catalog** - Comprehensive product database
- **Stock Levels** - Current quantity tracking
- **Reorder Levels** - Automatic low stock warnings
- **Pricing** - Base prices with tax calculations
- **SKU Management** - Unique stock codes

### 4. Sales Orders
- **Order Creation** - Customer order entry
- **Line Items** - Multiple products per order
- **BOM Integration** - Link BOMs to order items
- **Order Status** - Track order lifecycle
- **Customer Details** - Full customer information

### 5. Sales Invoices
- **Invoice Generation** - Create from sales orders
- **Tax Calculations** - Automatic VAT/sales tax
- **Payment Status** - Track paid/unpaid invoices
- **Invoice Details** - Complete invoice information
- **PDF Export** - Generate invoice documents (future enhancement)

### 6. Bill of Materials (BOM)
- **BOM Creation** - Define product assemblies
- **Component Management** - Track all components
- **Quantity Requirements** - Component quantities per BOM
- **Cost Rollup** - Calculate total BOM cost
- **Multi-level BOMs** - Support for sub-assemblies

### 7. Price Lists
- **Customer Pricing** - Special pricing for customers
- **Product Pricing** - Override default prices
- **Bulk Pricing** - Volume-based pricing
- **Effective Dates** - Time-based pricing rules

### 8. Invoice Payment
- **Payment Entry** - Record invoice payments
- **Multiple Payments** - Track partial payments
- **Payment Methods** - Support various payment types
- **Payment Allocation** - Apply payments to invoices

### 9. Management Dashboard
- **Sales Analytics** - Revenue and sales metrics
- **Inventory KPIs** - Stock levels and turnover
- **Financial Metrics** - Profit and cash flow
- **Charts and Graphs** - Visual data representation

### 10. Settings & Configuration
- **System Settings** - Global configuration
- **Company Details** - Organization information
- **Tax Configuration** - Tax rates and settings
- **User Preferences** - Personal settings

---

## 📸 Screenshots

Detailed screenshots of all features are available in the [`docs/screenshots/blazor/`](docs/screenshots/blazor/) directory.

### Available Screenshots

1. **Home Dashboard** - Main landing page
2. **Bank Accounts** - Account management interface
3. **Stock Items** - Product catalog
4. **Sales Orders** - Order processing
5. **Sales Invoices** - Invoice management
6. **Bill of Materials** - BOM management
7. **Price Lists** - Pricing management
8. **Invoice Payment** - Payment processing
9. **Management Dashboard** - Analytics and reporting
10. **Settings** - Configuration screens

For instructions on capturing screenshots, see the [Screenshots Guide](docs/SCREENSHOTS_GUIDE.md).

---

## ⚙️ Installation

### Prerequisites
- .NET 10 SDK or later
- Any modern IDE (Visual Studio 2022, VS Code, Rider)
- Web browser (Chrome, Firefox, Edge, Safari)

### Quick Start

1. **Clone the repository**
   ```bash
   git clone https://github.com/dotnetappdev/accountspoc.git
   cd accountspoc
   ```

2. **Start the Web API**
   ```bash
   cd src/AccountsPOC.WebAPI
   dotnet run
   ```
   The API will start at `http://localhost:5001`

3. **Start the Blazor App** (in a new terminal)
   ```bash
   cd src/AccountsPOC.BlazorApp
   dotnet run
   ```
   The app will start at `http://localhost:5193`

4. **Access the Application**
   Open your browser and navigate to `http://localhost:5193`

### Build the Solution

```bash
# Build all projects
dotnet build AccountsPOC.sln

# Or build specific project
cd src/AccountsPOC.BlazorApp
dotnet build
```

---

## 🔧 Configuration

### API Connection

The Blazor app communicates with the Web API. Configure the API URL in `Program.cs`:

```csharp
builder.Services.AddHttpClient("AccountsPOCAPI", client =>
{
    client.BaseAddress = new Uri("http://localhost:5001/");
});
```

For production deployment, update to your actual API URL:

```csharp
client.BaseAddress = new Uri("https://your-api-domain.com/");
```

### Database

The application uses SQLite by default. The database file `AccountsPOC.db` is created automatically in the Web API project directory on first run.

For SQL Server:
1. Update connection string in `appsettings.json`
2. Change `UseSqlite()` to `UseSqlServer()` in Web API's `Program.cs`
3. Run migrations: `dotnet ef database update`

### CORS Settings

The Web API is configured to allow requests from the Blazor app. Update CORS policy in Web API's `Program.cs` if deploying to different domains:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorApp", policy =>
    {
        policy.WithOrigins("http://localhost:5193", "https://your-blazor-domain.com")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

---

## 📖 Usage Guide

### Getting Started

1. **Start with Home Dashboard**
   - After launching, you'll see the home dashboard
   - Use the sidebar navigation to access features
   - Toggle between light/dark mode with the theme button

2. **Set Up Bank Accounts**
   - Navigate to "Bank Accounts"
   - Click "Add Bank Account"
   - Enter account details (name, number, balance)
   - Save to create the account

3. **Add Products**
   - Navigate to "Stock Items" or "Products"
   - Click "Add Product"
   - Enter product details (code, name, price, stock)
   - Set reorder levels for automatic warnings
   - Save the product

4. **Create Sales Orders**
   - Navigate to "Sales Orders"
   - Click "Create Order"
   - Enter customer details
   - Add line items (products and quantities)
   - Optionally link BOMs to line items
   - Save the order

5. **Generate Invoices**
   - Navigate to "Sales Invoices"
   - Click "Create Invoice"
   - Select a sales order
   - Review invoice details
   - Save to generate the invoice

6. **Process Payments**
   - Navigate to "Invoice Payment"
   - Select an invoice
   - Enter payment details
   - Apply payment to invoice

### Working with BOMs

1. **Create a BOM**
   - Navigate to "Bill of Materials"
   - Click "Create BOM"
   - Enter BOM name and description
   - Add components with quantities
   - Save the BOM

2. **Link BOM to Sales Order**
   - When creating a sales order item
   - Select the BOM from the dropdown
   - System tracks component requirements

### Price Lists

1. **Create Price List**
   - Navigate to "Price Lists"
   - Click "Create Price List"
   - Enter name and customer
   - Add products with special prices
   - Set effective dates

2. **Apply Price List**
   - When creating sales orders
   - System automatically applies customer's price list
   - Shows special pricing on order items

---

## 🔌 API Integration

The Blazor app consumes REST APIs from the AccountsPOC.WebAPI project. All data operations use HTTP requests.

### API Endpoints Used

- **GET** `/api/BankAccounts` - Fetch bank accounts
- **POST** `/api/BankAccounts` - Create new account
- **PUT** `/api/BankAccounts/{id}` - Update account
- **DELETE** `/api/BankAccounts/{id}` - Delete account

Similar patterns for:
- `/api/Products`
- `/api/SalesOrders`
- `/api/SalesInvoices`
- `/api/BillOfMaterials`
- `/api/PriceLists`

### Error Handling

The app includes error handling for:
- Network failures
- API errors
- Validation errors
- Unauthorized access

Errors are displayed using Bootstrap alerts and modal dialogs.

---

## 🏗️ Architecture

### Technology Stack

- **Blazor Server** - Interactive server-side rendering
- **Bootstrap 5** - Modern responsive UI
- **HttpClient** - API communication
- **Dependency Injection** - Service management
- **Razor Components** - Reusable UI components

### Project Structure

```
AccountsPOC.BlazorApp/
├── Components/
│   ├── Layout/
│   │   ├── MainLayout.razor      # Main layout with sidebar
│   │   ├── NavMenu.razor         # Navigation menu
│   │   └── ModernNavMenu.razor   # Modern styled menu
│   ├── Pages/
│   │   ├── Home.razor            # Home dashboard
│   │   ├── BankAccounts.razor    # Bank account management
│   │   ├── StockItems.razor      # Product catalog
│   │   ├── SalesOrders.razor     # Order processing
│   │   ├── SalesInvoices.razor   # Invoice management
│   │   └── ...                   # Other pages
│   └── Shared/
│       ├── ModalDialog.razor     # Reusable modal
│       └── CurrencyInput.razor   # Currency input control
├── wwwroot/                      # Static assets
├── Program.cs                     # App configuration
└── appsettings.json              # Configuration settings
```

### Component Architecture

- **Pages** - Route-able components with @page directive
- **Layouts** - Shared layout components
- **Shared Components** - Reusable UI elements
- **Services** - HttpClient services for API calls

---

## 🔍 Troubleshooting

### Common Issues

#### API Connection Failed
**Problem**: "Failed to connect to API" error
**Solution**: 
1. Ensure Web API is running on `http://localhost:5001`
2. Check CORS settings in Web API
3. Verify API base URL in Blazor app's configuration

#### Database Errors
**Problem**: "Database not found" or "Table doesn't exist"
**Solution**:
1. Stop the Web API
2. Delete the `AccountsPOC.db` file
3. Restart Web API to recreate the database
4. Or run migrations: `dotnet ef database update`

#### Port Already in Use
**Problem**: "Port 5193 is already in use"
**Solution**:
1. Change port in `launchSettings.json`
2. Or stop the process using the port
3. Or run with `dotnet run --urls "http://localhost:5194"`

#### Reconnection Issues
**Problem**: "Attempting to reconnect" message
**Solution**:
1. Check browser console for errors
2. Ensure stable network connection
3. Refresh the page
4. Restart the Blazor app

### Performance Tips

1. **Use pagination** for large data sets
2. **Implement caching** for frequently accessed data
3. **Optimize images** in wwwroot folder
4. **Enable compression** in production
5. **Use CDN** for Bootstrap and other libraries

### Development Tips

1. **Hot Reload** - Use `dotnet watch run` for automatic refresh
2. **Browser DevTools** - Use F12 to debug JavaScript issues
3. **Logging** - Check application logs for errors
4. **Network Tab** - Monitor API calls in browser DevTools

---

## 📚 Additional Resources

- [Main README](README.md) - Project overview
- [Mobile App Guide](MOBILE_APP_GUIDE.md) - Mobile app documentation
- [MAUI README](MAUI_README.md) - MAUI implementation details
- [Screenshots Guide](docs/SCREENSHOTS_GUIDE.md) - Screenshot documentation
- [Implementation Summary](IMPLEMENTATION_SUMMARY.md) - Technical details

## 🤝 Contributing

Contributions are welcome! Please:
1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Submit a pull request

## 📄 License

This is a proof-of-concept application for demonstration purposes.

---

**Note**: This is a comprehensive accounting system built for demonstration and learning purposes. For production use, additional features like authentication, authorization, audit logging, and data backup should be implemented.
