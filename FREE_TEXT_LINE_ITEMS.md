# Free-Text Line Item Feature

## Overview
Added support for free-text and non-stock line items to Sales Orders, allowing users to add custom items that aren't in the product catalog.

## Features

### User Interface
- **Item Type Toggle**: Radio buttons to switch between "Product" and "Free Text / Non-Stock Item" modes
- **Product Mode**: 
  - Select from product dropdown
  - Auto-fill unit price from product
  - Quantity and editable price fields
- **Free Text Mode**:
  - Custom description field for item name
  - Quantity and unit price fields (no auto-fill)
  - Support for services, custom items, or non-inventory items

### Grid Display
- Products display with product name and code
- Free-text items display with:
  - Pencil icon (🖊️) to indicate custom item
  - Custom description instead of product name
  - Dash (-) for code column

### API Validation
- Product items: Validates product exists in database
- Free-text items: Validates description is provided
- Both types: Validates quantity > 0 and unit price >= 0

## Usage Scenarios

### Example: Adding a Product
1. Keep "Product" radio button selected (default)
2. Select product from dropdown
3. Enter quantity
4. Price auto-fills from product (editable)
5. Click "Add Item"

### Example: Adding a Free Text Item
1. Select "Free Text / Non-Stock Item" radio button
2. Enter description (e.g., "Installation Service", "Custom Fabrication", "Delivery Fee")
3. Enter quantity
4. Enter unit price
5. Click "Add Item"

## Technical Implementation

### Database Schema
Already supported in `SalesOrderItem` entity:
- `ProductId` (nullable) - Null for free-text items
- `Description` - Used for free-text item names
- `IsFreeTextItem` - Boolean flag to distinguish item types

### Files Modified
1. `SalesOrders.razor` - Added UI toggle and free-text input
2. `SalesOrdersController.cs` - Added validation logic for both item types

## Benefits
- ✅ Support for services and non-inventory items
- ✅ Flexibility for one-off custom items
- ✅ No need to create products for every line item
- ✅ Faster order entry for miscellaneous items
- ✅ Clear visual distinction in the grid
