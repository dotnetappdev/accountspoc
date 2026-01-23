# Sage 200 Forms Configuration Guide

This document describes the Sage 200-style form improvements implemented in the AccountsPOC application.

## Overview

The application has been updated to provide a more comprehensive, Sage 200-like experience with:
- Enhanced stock item management with all Sage 200 fields
- Modal dialog forms for better UX
- Reusable pagination component
- Comprehensive system configuration options

## Stock Item Management

### New Fields Added

The Stock Item entity now includes all fields typical of Sage 200:

#### General Information
- **Stock Code**: Unique identifier for the stock item
- **Description**: Short description
- **Long Description**: Detailed description
- **Category**: Product category classification
- **Barcode**: Product barcode/EAN
- **Manufacturer Part Number**: Manufacturer's reference
- **Internal Reference**: Company-specific reference
- **Notes**: Additional notes

#### Units of Measure
- **Unit of Measure**: Primary unit (Each, Box, Pallet, etc.)
- **Alternative Unit of Measure**: Secondary unit
- **Units Per Pack**: Conversion factor between units

#### Pricing & Tax
- **Cost Price**: Purchase cost
- **Selling Price**: Sales price
- **Tax Code**: Tax classification code
- **Account Code**: General ledger account code
- **Tax Exempt**: Flag for tax-exempt items

#### Stock Control
- **Quantity On Hand**: Current physical stock
- **Quantity Allocated**: Stock allocated to orders
- **Quantity On Order**: Stock on purchase orders
- **Quantity Available**: Calculated (On Hand - Allocated)
- **Reorder Level**: Minimum stock trigger
- **Reorder Quantity**: Standard reorder amount
- **Warehouse ID**: Primary warehouse location
- **Bin Location**: Specific location within warehouse

#### Supplier Information
- **Default Supplier ID**: Primary supplier reference
- **Supplier Part Number**: Supplier's product code
- **Supplier Lead Time Days**: Expected delivery time

#### Dimensions & Shipping
- **Weight**: Product weight
- **Weight UOM**: Unit (kg, lbs, etc.)
- **Length, Width, Height**: Product dimensions
- **Dimension UOM**: Unit (cm, inches, etc.)

#### Status Flags
- **Is Active**: Product is currently available
- **Is Discontinued**: Product is discontinued
- **Discontinued Date**: When product was discontinued
- **Is Age Restricted**: Requires age verification
- **Minimum Age**: Required minimum age
- **Requires OTP Verification**: Additional verification needed

## System Configuration

### Pagination Settings

Configure pagination behavior in **SystemSettings**:

```csharp
// Default pagination
DefaultPageSize = 25              // Default items per page
MaxPageSize = 100                 // Maximum items per page
PaginationSizes = "10,25,50,100"  // Available page size options

// Pagination display options
ShowPaginationInfo = true         // Show "Showing X to Y of Z entries"
ShowPageNumbers = true            // Show numbered page buttons
MaxPageNumbers = 5                // Maximum page number buttons displayed
```

### File Upload Settings

Configure file upload behavior in **SystemSettings**:

```csharp
AllowMultipleFileUploads = true         // Enable multiple file selection
MaxFileUploadCount = 10                 // Maximum files per upload
MaxFileUploadSizeMB = 10                // Maximum file size in MB
AllowedFileExtensions = ".pdf,.doc,.docx,.xls,.xlsx,.jpg,.jpeg,.png,.gif"
```

## Using the Pagination Component

The reusable Pagination component can be added to any page:

```razor
<Pagination 
    CurrentPage="currentPage"
    PageSize="pageSize"
    TotalCount="totalCount"
    PageSizes="pageSizes"
    OnPageChanged="HandlePageChanged"
    PageSizeChanged="HandlePageSizeChanged"
    ShowInfo="true"
    ShowPageNumbers="true"
    ShowPageSizeDropdown="true" 
    MaxPageNumbers="5" />
```

### Component Parameters

- **CurrentPage**: Current page number (1-based)
- **PageSize**: Number of items per page
- **TotalCount**: Total number of items
- **PageSizes**: List of available page sizes (e.g., `new() { 10, 25, 50, 100 }`)
- **OnPageChanged**: Event callback when page changes
- **PageSizeChanged**: Event callback when page size changes
- **ShowInfo**: Display "Showing X to Y of Z entries" (default: true)
- **ShowPageNumbers**: Display numbered page buttons (default: true)
- **ShowPageSizeDropdown**: Display page size selector (default: true)
- **MaxPageNumbers**: Maximum page number buttons to display (default: 5)

## Modal Dialog Forms

### Usage Pattern

Forms now use the `ModalDialog` component for a better user experience:

```razor
<ModalDialog Title="Add New Stock Item"
             IsVisible="showForm"
             OnClose="CancelForm"
             Size="lg">
    <Body>
        <EditForm Model="currentItem" OnValidSubmit="SaveItem">
            <DataAnnotationsValidator />
            <!-- Form fields here -->
        </EditForm>
    </Body>
</ModalDialog>
```

### Modal Sizes

- **sm**: Small modal (500px max width)
- **md**: Medium modal - default (800px max width)
- **lg**: Large modal (1000px max width)
- **xl**: Extra large modal (1140px max width)

## ViewModel Pattern

Forms use view models instead of entities directly:

```csharp
// In code section
private class StockItemViewModel
{
    // ViewModel properties matching form needs
    public string StockCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    // ... other properties
}

// Map ViewModel to Entity before saving
private StockItem MapToEntity(StockItemViewModel vm)
{
    return new StockItem
    {
        Id = vm.Id,
        StockCode = vm.StockCode,
        Description = vm.Description,
        // ... other mappings
    };
}
```

## Form Validation

Validation is enabled using `DataAnnotationsValidator`:

```razor
<EditForm Model="currentItem" OnValidSubmit="SaveItem">
    <DataAnnotationsValidator />
    
    <label class="form-label">
        Stock Code <span class="text-danger">*</span>
    </label>
    <InputText @bind-Value="currentItem.StockCode" />
    <ValidationMessage For="@(() => currentItem.StockCode)" />
</EditForm>
```

## Best Practices

1. **Use ViewModels**: Don't bind entities directly to forms
2. **Organize Forms**: Group related fields into logical sections
3. **Add Validation**: Use DataAnnotations for required fields
4. **Use Modals**: Provide consistent UX with modal dialogs
5. **Enable Pagination**: Use the pagination component for lists
6. **Configure Settings**: Set appropriate pagination and upload limits

## Migration Notes

The database uses `EnsureCreated()` so schema changes are automatically applied on application restart. For production environments, consider using EF Core migrations:

```bash
dotnet ef migrations add AddSageFieldsToStockItem
dotnet ef database update
```

## Future Enhancements

Potential improvements:
- Add tabbed interface for complex forms (General, Pricing, Stock, Supplier, Dimensions, Custom Fields)
- Implement server-side pagination for large datasets
- Add advanced filtering and search capabilities
- Integrate custom fields dynamically
- Add bulk operations support
