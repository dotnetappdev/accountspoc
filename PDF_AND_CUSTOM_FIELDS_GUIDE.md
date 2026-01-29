# PDF Support and Custom Fields Implementation Guide

## Overview

This document describes the PDF generation functionality and custom fields enhancement implemented in the AccountsPOC system.

## New Features

### 1. PDF Generation Library (AccountsPOC.PdfGenerator)

A new library has been created to generate professional PDFs with tenant branding support using QuestPDF.

#### Components:

- **BasePdfTemplate**: Abstract base class for all PDF templates with tenant branding
- **SalesOrderPdfTemplate**: Template for Sales Order PDFs
- **SalesInvoicePdfTemplate**: Template for Sales Invoice PDFs
- **PartialDispatchPdfTemplate**: Template for Dispatch Note PDFs
- **PdfGeneratorService**: Service for generating PDFs

#### Features:

- Tenant branding (logo, name, address, contact info)
- Professional layouts with headers and footers
- Line items with descriptions, quantities, prices
- Totals with tax calculations
- Status badges and formatting

### 2. Enhanced Entity Models

#### New Entities:

- **SalesInvoiceItem**: Line items for sales invoices with free-text support
- **PartialDispatch**: Tracks partial dispatches of sales orders
- **PartialDispatchItem**: Line items for partial dispatches

#### Enhanced Entities:

- **SalesOrderItem**: Added free-text line item support
  - `Description` field for custom text
  - `IsFreeTextItem` flag
  - `LineNumber` for ordering
  - `Notes` for additional information
  - `ProductId` is now nullable to support free-text items

- **SalesInvoice**: Added collection of SalesInvoiceItems

### 3. Custom Fields Enhancement

Custom fields can now be attached to additional entity types:

- SalesOrderItem (document line items)
- SalesInvoiceItem (invoice line items)
- StockItem
- Product

The CustomField entity has been updated to support these new entity types.

### 4. API Endpoints

#### Sales Orders
- `GET /api/SalesOrders/{id}/pdf` - Download Sales Order as PDF

#### Sales Invoices
- `GET /api/SalesInvoices/{id}/pdf` - Download Sales Invoice as PDF

#### Partial Dispatches
- `GET /api/PartialDispatches` - List all partial dispatches
- `GET /api/PartialDispatches/{id}` - Get specific partial dispatch
- `POST /api/PartialDispatches` - Create new partial dispatch
- `PUT /api/PartialDispatches/{id}` - Update partial dispatch
- `DELETE /api/PartialDispatches/{id}` - Delete partial dispatch
- `GET /api/PartialDispatches/{id}/pdf` - Download Dispatch Note as PDF

### 5. UI Updates

#### Sales Orders Page
- Added "Download PDF" button (PDF icon) in the Actions column
- Downloads a professionally formatted PDF of the sales order

#### Sales Invoices Page
- Added "Download PDF" button (PDF icon) in the invoice card footer
- Downloads a professionally formatted PDF of the invoice

#### Custom Fields Page
- Added new entity type tabs:
  - Sales Invoice Items
  - Stock Items
  - Products

#### JavaScript Helpers
- New file: `wwwroot/js/file-download.js`
- Provides `downloadFile()` function for triggering browser downloads

## Usage

### Generating PDFs

PDFs are automatically generated when users click the PDF download buttons in the UI. The system:

1. Retrieves the entity data (order, invoice, or dispatch)
2. Fetches tenant branding information
3. Generates the PDF using QuestPDF templates
4. Returns the PDF as a downloadable file

### Adding Free-Text Line Items

When creating sales orders or invoices, users can now add free-text line items:

1. Set `IsFreeTextItem = true`
2. Set `ProductId = null`
3. Provide a `Description` for the line item
4. Set `Quantity`, `UnitPrice`, and `TotalPrice` as needed

### Attaching Custom Fields

Custom fields can now be attached to:
- Sales Order Items
- Sales Invoice Items
- Stock Items
- Products

Use the Custom Fields management page to:
1. Select the entity type from the tabs
2. Create new custom fields for that entity type
3. Set field properties (name, label, type, required, etc.)

The custom field values can be stored using the existing CustomFieldValue entity:
```csharp
var customFieldValue = new CustomFieldValue
{
    CustomFieldId = fieldId,
    EntityType = "SalesOrderItem",
    EntityId = salesOrderItemId,
    FieldValue = "Custom value",
    CreatedDate = DateTime.UtcNow
};
```

## Configuration

### QuestPDF License

The system uses QuestPDF Community license which is free for non-commercial use. This is configured in `PdfGeneratorService`:

```csharp
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
```

For commercial use, you'll need to purchase a QuestPDF license.

### Tenant Branding

Tenant branding is retrieved from:
- **Tenant** table: Company name, phone, email
- **BrandingAssets** table: Logo image (AssetType = "Logo")

The branding information is automatically included in all generated PDFs.

## Database Schema

### New Tables:

1. **SalesInvoiceItems**
   - Id (int, PK)
   - SalesInvoiceId (int, FK)
   - ProductId (int?, nullable, FK)
   - Description (string, nullable)
   - IsFreeTextItem (bool)
   - Quantity (int)
   - UnitPrice (decimal)
   - TotalPrice (decimal)
   - TaxRate (decimal)
   - TaxAmount (decimal)
   - LineNumber (int)
   - Notes (string, nullable)

2. **PartialDispatches**
   - Id (int, PK)
   - TenantId (int, FK)
   - SalesOrderId (int, FK)
   - DispatchNumber (string)
   - DispatchDate (datetime)
   - CarrierName (string, nullable)
   - TrackingNumber (string, nullable)
   - ShippingMethod (string, nullable)
   - Weight (decimal)
   - WeightUnit (string, nullable)
   - NumberOfPackages (int)
   - Status (string)
   - Delivery address fields
   - Notes (string, nullable)
   - Dates and audit fields

3. **PartialDispatchItems**
   - Id (int, PK)
   - PartialDispatchId (int, FK)
   - SalesOrderItemId (int, FK)
   - QuantityDispatched (int)
   - Notes (string, nullable)

### Updated Tables:

**SalesOrderItems** - Added columns:
- ProductId (changed to nullable)
- Description (string, nullable)
- IsFreeTextItem (bool)
- LineNumber (int)
- Notes (string, nullable)

## Technical Implementation

### Dependencies

- **QuestPDF** (v2024.12.3): PDF generation library
- **AccountsPOC.Domain**: Domain entities
- **Microsoft.EntityFrameworkCore**: Data access

### Service Registration

In `Program.cs`:
```csharp
builder.Services.AddScoped<IPdfGeneratorService, PdfGeneratorService>();
```

### PDF Template Structure

All PDF templates inherit from `BasePdfTemplate` which provides:
- Header with logo and tenant information
- Footer with page numbers
- Common styling and formatting

Each specific template implements:
- `Compose(IDocumentContainer container)` method
- Custom content layout
- Specific data rendering

## Future Enhancements

Potential improvements for future releases:

1. **Email Integration**: Send PDFs directly via email
2. **Custom Templates**: Allow users to customize PDF templates
3. **Additional Documents**: Purchase Orders, Packing Lists, etc.
4. **Batch Generation**: Generate multiple PDFs at once
5. **PDF Storage**: Store generated PDFs for later retrieval
6. **Digital Signatures**: Add support for digital signatures
7. **Watermarks**: Add watermark support for draft documents

## Support

For issues or questions about the PDF generation or custom fields functionality, please contact the development team or refer to the main project documentation.
