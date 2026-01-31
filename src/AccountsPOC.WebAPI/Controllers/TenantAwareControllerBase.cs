using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AccountsPOC.WebAPI.Controllers;

/// <summary>
/// Base controller that provides tenant context for multi-tenant applications.
/// All controllers that need tenant isolation should inherit from this.
/// </summary>
[ApiController]
public abstract class TenantAwareControllerBase : ControllerBase
{
    /// <summary>
    /// Gets the current tenant ID from the authenticated user's claims.
    /// Returns null if the user is not authenticated or tenant claim is not found.
    /// </summary>
    protected int? CurrentTenantId
    {
        get
        {
            var tenantClaim = User?.FindFirst("tenant_id")?.Value;
            if (int.TryParse(tenantClaim, out var tenantId))
            {
                return tenantId;
            }
            return null;
        }
    }

    /// <summary>
    /// Gets the current tenant ID or throws UnauthorizedAccessException if not available.
    /// Use this when tenant context is required.
    /// </summary>
    protected int GetRequiredTenantId()
    {
        var tenantId = CurrentTenantId;
        if (!tenantId.HasValue)
        {
            throw new UnauthorizedAccessException("Tenant context is required but not found in user claims.");
        }
        return tenantId.Value;
    }

    /// <summary>
    /// Validates that the specified tenant ID matches the current user's tenant.
    /// Returns 403 Forbidden if there's a mismatch.
    /// </summary>
    protected ActionResult ValidateTenantAccess(int requestedTenantId)
    {
        var currentTenantId = GetRequiredTenantId();
        if (currentTenantId != requestedTenantId)
        {
            return Forbid("Access denied: You do not have permission to access resources from another tenant.");
        }
        return Ok();
    }

    /// <summary>
    /// Checks if the current user has access to the specified tenant.
    /// </summary>
    protected bool HasTenantAccess(int tenantId)
    {
        var currentTenantId = CurrentTenantId;
        return currentTenantId.HasValue && currentTenantId.Value == tenantId;
    }
}
