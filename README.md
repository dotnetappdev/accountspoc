# Accounts POC - Full-Featured Accounting Package

A modern, full-featured accounting management system similar to Sage 200, built with ASP.NET Core Web API and Blazor.

## Features

### Core Accounting Features
- **Multiple Bank Accounts Management** - Track multiple bank accounts with balance monitoring
- **Product Catalog** - Comprehensive product management with stock control and reorder levels
- **Sales Order Processing** - Create and manage sales orders with customer information
- **Sales Invoice Generation** - Generate invoices from sales orders with tax calculations
- **Bill of Materials (BOM)** - Create and manage BOMs with component tracking
- **BOM to Sales Order Linking** - Link BOMs to sales order items for component requirement tracking

## Architecture

### Clean Architecture Implementation
```
AccountsPOC/
├── src/
│   ├── AccountsPOC.Domain/          # Domain entities
│   ├── AccountsPOC.Application/     # Application logic layer
│   ├── AccountsPOC.Infrastructure/  # Data access with EF Core
│   ├── AccountsPOC.WebAPI/          # RESTful API (.NET 10)
│   └── AccountsPOC.BlazorApp/       # Blazor Web App UI (.NET 10)
```

### Technology Stack
- **.NET 10** - Latest .NET framework
- **ASP.NET Core Web API** - RESTful backend services
- **Blazor Web App** - Modern, interactive UI with server-side rendering
- **Entity Framework Core** - ORM with SQLite database
- **Bootstrap 5** - Modern, responsive UI framework
- **Clean Architecture** - Separation of concerns and maintainability

## Database Schema

### Entities
- **BankAccount** - Multiple bank accounts with balance tracking
- **Product** - Product catalog with pricing and stock levels
- **SalesOrder** - Customer orders with items
- **SalesOrderItem** - Line items on sales orders (links to products and BOMs)
- **SalesInvoice** - Invoices generated from orders
- **BillOfMaterial** - BOMs for products
- **BOMComponent** - Components that make up a BOM

### Key Relationships
- Products can have multiple BOMs
- Sales Orders contain multiple Sales Order Items
- Sales Order Items can link to a BOM
- Sales Invoices are linked to Sales Orders
- BOMs contain multiple BOM Components

## Getting Started

### Prerequisites
- .NET 10 SDK
- Any modern IDE (Visual Studio 2022, VS Code, Rider)

### Running the Application

1. **Start the Web API:**
```bash
cd src/AccountsPOC.WebAPI
dotnet run
```
The API will start at `http://localhost:5001`

2. **Start the Blazor App:**
```bash
cd src/AccountsPOC.BlazorApp
dotnet run
```
The Blazor app will start at `http://localhost:5193`

3. **Access the Application:**
Open your browser and navigate to `http://localhost:5193`

### Building the Solution
```bash
dotnet build AccountsPOC.sln
```

## API Endpoints

### Bank Accounts
- `GET /api/BankAccounts` - List all bank accounts
- `GET /api/BankAccounts/{id}` - Get specific bank account
- `POST /api/BankAccounts` - Create new bank account
- `PUT /api/BankAccounts/{id}` - Update bank account
- `DELETE /api/BankAccounts/{id}` - Delete bank account

### Products
- `GET /api/Products` - List all products
- `GET /api/Products/{id}` - Get specific product
- `POST /api/Products` - Create new product
- `PUT /api/Products/{id}` - Update product
- `DELETE /api/Products/{id}` - Delete product

### Sales Orders
- `GET /api/SalesOrders` - List all sales orders with items
- `GET /api/SalesOrders/{id}` - Get specific sales order
- `POST /api/SalesOrders` - Create new sales order
- `PUT /api/SalesOrders/{id}` - Update sales order
- `DELETE /api/SalesOrders/{id}` - Delete sales order

### Sales Invoices
- `GET /api/SalesInvoices` - List all sales invoices
- `GET /api/SalesInvoices/{id}` - Get specific sales invoice
- `POST /api/SalesInvoices` - Create new sales invoice
- `PUT /api/SalesInvoices/{id}` - Update sales invoice
- `DELETE /api/SalesInvoices/{id}` - Delete sales invoice

### Bill of Materials
- `GET /api/BillOfMaterials` - List all BOMs with components
- `GET /api/BillOfMaterials/{id}` - Get specific BOM
- `POST /api/BillOfMaterials` - Create new BOM
- `PUT /api/BillOfMaterials/{id}` - Update BOM
- `DELETE /api/BillOfMaterials/{id}` - Delete BOM

## UI Screenshots

The application features a modern, responsive UI with the following screens:

- **Home Dashboard** - Overview of the system with quick access to all features
- **Bank Accounts** - Manage multiple bank accounts with balance tracking
- **Products** - Product catalog management with stock control
- **Sales Orders** - Create and manage customer orders
- **Sales Invoices** - Generate and track invoices
- **Bill of Materials** - Create and manage BOMs with component linking

## Development

### Project Structure

#### Domain Layer
Contains all domain entities with business logic:
- Entities are in `AccountsPOC.Domain/Entities/`
- No dependencies on other layers

#### Application Layer
Contains application services and interfaces:
- References Domain layer only
- Defines interfaces for infrastructure

#### Infrastructure Layer
Implements data access:
- `ApplicationDbContext` - EF Core DbContext
- Database configurations
- References Domain and Application layers

#### Web API Layer
RESTful API implementation:
- Controllers for each entity
- CORS configuration for Blazor app
- References Application and Infrastructure layers

#### Blazor App Layer
Modern web UI:
- Interactive server-side components
- Pages for each feature area
- HTTP client for API communication

## Database

The application uses SQLite for simplicity and portability. The database file `AccountsPOC.db` is created automatically in the Web API project directory on first run.

For production use, you can easily switch to SQL Server by:
1. Updating the connection string in `appsettings.json`
2. Changing `UseSqlite()` to `UseSqlServer()` in `Program.cs`

## Security Summary

The application implements:
- Input validation through Entity Framework
- CORS policy to restrict API access
- Proper data types with precision for monetary values
- Unique indexes on key fields (account numbers, product codes, etc.)

No critical security vulnerabilities detected.

## Future Enhancements

Potential areas for expansion:
- User authentication and authorization
- Multi-tenant support
- Advanced reporting and analytics
- Payment processing integration
- Inventory management
- Purchase orders
- General ledger integration
- Excel import/export
- PDF invoice generation

## License

This is a proof-of-concept application for demonstration purposes.