# IDOR Protection Implementation Guide

## Overview
This document describes the IDOR (Insecure Direct Object Reference) protection implementation for the multi-tenant AccountsPOC application.

## Problem
The application previously did not enforce tenant isolation in API controllers, allowing users from one tenant to potentially access or modify data from another tenant by guessing IDs.

## Solution
We've implemented a `TenantAwareControllerBase` base controller that:
1. Extracts the tenant ID from the authenticated user's JWT claims
2. Provides helper methods for tenant validation
3. Ensures all data operations are scoped to the current user's tenant

## Implementation Pattern

### Base Controller
All controllers that handle tenant-specific data should inherit from `TenantAwareControllerBase`:

```csharp
public class MyController : TenantAwareControllerBase
{
    // Controller implementation
}
```

### GET All Items
```csharp
[HttpGet]
public async Task<ActionResult<IEnumerable<Entity>>> GetEntities()
{
    var tenantId = GetRequiredTenantId();
    return await _context.Entities
        .Where(e => e.TenantId == tenantId)
        .ToListAsync();
}
```

### GET Single Item
```csharp
[HttpGet("{id}")]
public async Task<ActionResult<Entity>> GetEntity(int id)
{
    var tenantId = GetRequiredTenantId();
    var entity = await _context.Entities
        .FirstOrDefaultAsync(e => e.Id == id && e.TenantId == tenantId);

    if (entity == null)
    {
        return NotFound();
    }

    return entity;
}
```

### POST (Create)
```csharp
[HttpPost]
public async Task<ActionResult<Entity>> PostEntity(Entity entity)
{
    var tenantId = GetRequiredTenantId();
    
    // Force the entity to belong to the current tenant
    entity.TenantId = tenantId;
    entity.CreatedDate = DateTime.UtcNow;
    
    _context.Entities.Add(entity);
    await _context.SaveChangesAsync();

    return CreatedAtAction(nameof(GetEntity), new { id = entity.Id }, entity);
}
```

### PUT (Update)
```csharp
[HttpPut("{id}")]
public async Task<IActionResult> PutEntity(int id, Entity entity)
{
    if (id != entity.Id)
    {
        return BadRequest();
    }

    var tenantId = GetRequiredTenantId();
    
    // Verify the entity belongs to the current tenant
    var existing = await _context.Entities
        .FirstOrDefaultAsync(e => e.Id == id && e.TenantId == tenantId);
    
    if (existing == null)
    {
        return NotFound();
    }

    // Ensure tenant ID cannot be changed
    entity.TenantId = tenantId;
    entity.LastModifiedDate = DateTime.UtcNow;
    _context.Entry(entity).State = EntityState.Modified;

    try
    {
        await _context.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)
    {
        if (!await EntityExistsForTenant(id, tenantId))
        {
            return NotFound();
        }
        throw;
    }

    return NoContent();
}
```

### DELETE
```csharp
[HttpDelete("{id}")]
public async Task<IActionResult> DeleteEntity(int id)
{
    var tenantId = GetRequiredTenantId();
    var entity = await _context.Entities
        .FirstOrDefaultAsync(e => e.Id == id && e.TenantId == tenantId);
    
    if (entity == null)
    {
        return NotFound();
    }

    _context.Entities.Remove(entity);
    await _context.SaveChangesAsync();

    return NoContent();
}
```

## Controllers Updated
The following controllers have been updated with IDOR protection:
- ✅ CustomersController
- ✅ BankAccountsController
- ✅ TenantsController (with license limit check)

## Controllers That Need Updates
The following controllers need to be reviewed and updated following the pattern above:

### High Priority (User-facing data)
- ProductsController
- SalesOrdersController
- SalesInvoicesController
- StockItemsController
- WarehousesController
- QuotesController
- WorkOrdersController
- PriceListsController

### Medium Priority (Configuration data)
- ConfigurationSettingsController
- CustomFieldsController
- EmailTemplatesController
- ExchangeRatesController
- BrandingAssetsController
- PaymentProviderConfigsController

### Low Priority (System data)
- ApiKeyConfigController
- RolesController
- PermissionsController
- UsersController
- LicensesController
- InstallationsController

## Special Cases

### Controllers Without Tenant Context
Some controllers like `AuthController`, `SetupController`, and `DashboardController` may not need tenant filtering for all endpoints. Review these carefully.

### TenantsController
The TenantsController itself doesn't filter by tenant in GET operations because system admins may need to see all tenants. However, creation is protected by license limits.

## Testing
After updating a controller, test:
1. Users can only access their own tenant's data
2. Attempts to access other tenants' data return 404 or 403
3. Tenant ID cannot be changed through updates
4. Creating resources automatically assigns the correct tenant

## Security Notes
- Never trust client-provided tenant IDs in request bodies
- Always use `GetRequiredTenantId()` to get the tenant from JWT claims
- Filter ALL queries by tenant ID, including related entities
- Use `.FirstOrDefaultAsync()` with tenant check instead of `.FindAsync()`
