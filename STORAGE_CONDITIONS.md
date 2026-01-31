# Storage Condition Field for Stock Items

## Overview
Added a storage condition field to Stock Items to track special storage requirements for food, perishables, and items requiring specific environmental conditions.

## Features

### Dropdown Options
The Storage Condition dropdown includes:
- **Not Required** (default) - For materials and items with no special storage needs
- **Ambient / Room Temperature** - Standard storage
- **Refrigerated (2-8°C)** - For items requiring refrigeration
- **Frozen (-18°C or below)** - For frozen goods
- **Chilled (0-4°C)** - For fresh items needing cold but not frozen
- **Temperature Controlled** - For items with specific temperature requirements
- **Dry Storage** - For items requiring low humidity
- **Hazardous Materials** - For items requiring special handling
- **Perishable / Fresh** - For fresh produce and time-sensitive items

### Visual Indicators
The Stock Items grid displays icons for quick identification:
- ❄️ (Snowflake) - Refrigerated, Frozen, or Chilled items
- 🥗 (Salad) - Perishable items
- ⚠️ (Warning) - Hazardous materials
- 🌡️ (Thermometer) - Temperature controlled items
- 📦 (Box) - Other storage conditions

### Smart Alerts
When editing a stock item:
- If storage condition is set to Refrigerated, Frozen, Chilled, or Perishable, an informational alert displays
- Alert reminds users of the special storage requirements
- Helps prevent storage errors and compliance issues

## Usage

### When to Use Storage Conditions
**Required for:**
- Food items (refrigerated, frozen, or perishable)
- Pharmaceuticals requiring temperature control
- Chemicals with storage requirements
- Fresh produce
- Dairy products
- Meat and seafood

**Not required for:**
- General merchandise
- Hardware and tools
- Office supplies
- Non-perishable materials
- Standard inventory items

### How to Set Storage Condition
1. Open Stock Items page
2. Create new or edit existing stock item
3. Navigate to the **Inventory** tab
4. Locate the **Storage Condition** dropdown
5. Select appropriate storage type (or leave as "Not Required")
6. Save the stock item

### Grid Display
The Storage column in the Stock Items grid shows:
- Icon + Storage type for items with conditions
- Dash (-) for items without storage requirements
- Filterable and sortable for easy searching

## Benefits
- ✅ Ensures proper storage of temperature-sensitive items
- ✅ Helps with warehouse organization and zone assignment
- ✅ Supports compliance for food safety regulations
- ✅ Quick visual identification of special storage needs
- ✅ Optional field - doesn't clutter interface for non-applicable items
- ✅ Flexible dropdown accommodates various storage types

## Technical Implementation

### Database Schema
- **Field**: `StorageCondition` (nullable string)
- **Table**: `StockItems`
- **Migration**: `20260130235900_AddStorageConditionToStockItems`

### Files Modified
1. `StockItem.cs` (Domain entity) - Added StorageCondition property
2. `StockItem.cs` (Blazor model) - Added StorageCondition property
3. `StockItems.razor` - Added dropdown in Inventory tab and grid column with icons
4. Migration file created for database update

### Location in UI
- **Edit Form**: Inventory tab → Storage Condition dropdown (3rd column of Warehouse/Bin row)
- **Grid View**: Storage column (between Qty Available and Warehouse columns)
