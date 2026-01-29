using AccountsPOC.Domain.Entities;
using AccountsPOC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AccountsPOC.WebAPI.Services;

public interface ILicenseService
{
    Task<bool> CanAddStockItem(int tenantId);
    Task<bool> CanAddUser(int tenantId);
    Task<bool> CanAddCustomer(int tenantId);
    Task<bool> CanAddInstallation(int tenantId);
    Task<bool> CanAddStockItemImage(int stockItemId);
    Task<License?> GetActiveLicense(int tenantId);
    Task<LicenseValidationResult> ValidateLicense(int tenantId);
}

public class LicenseService : ILicenseService
{
    private readonly ApplicationDbContext _context;

    public LicenseService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<License?> GetActiveLicense(int tenantId)
    {
        return await _context.Licenses
            .Where(l => l.TenantId == tenantId && l.IsActive)
            .Where(l => l.ExpiryDate == null || l.ExpiryDate > DateTime.UtcNow)
            .OrderByDescending(l => l.ActivationDate)
            .FirstOrDefaultAsync();
    }

    public async Task<LicenseValidationResult> ValidateLicense(int tenantId)
    {
        var license = await GetActiveLicense(tenantId);
        
        if (license == null)
        {
            return new LicenseValidationResult
            {
                IsValid = false,
                Message = "No active license found"
            };
        }

        if (license.ExpiryDate.HasValue && license.ExpiryDate < DateTime.UtcNow)
        {
            return new LicenseValidationResult
            {
                IsValid = false,
                Message = "License has expired"
            };
        }

        return new LicenseValidationResult
        {
            IsValid = true,
            License = license
        };
    }

    public async Task<bool> CanAddStockItem(int tenantId)
    {
        var license = await GetActiveLicense(tenantId);
        if (license == null) return false;

        if (license.MaxStockItems == null) return true; // Unlimited

        var currentCount = await _context.StockItems.CountAsync(s => s.TenantId == tenantId);
        return currentCount < license.MaxStockItems;
    }

    public async Task<bool> CanAddUser(int tenantId)
    {
        var license = await GetActiveLicense(tenantId);
        if (license == null) return false;

        if (license.MaxUsers == null) return true; // Unlimited

        var currentCount = await _context.Users.CountAsync(u => u.TenantId == tenantId);
        return currentCount < license.MaxUsers;
    }

    public async Task<bool> CanAddCustomer(int tenantId)
    {
        var license = await GetActiveLicense(tenantId);
        if (license == null) return false;

        if (license.MaxCustomers == null) return true; // Unlimited

        var currentCount = await _context.Customers.CountAsync(c => c.TenantId == tenantId);
        return currentCount < license.MaxCustomers;
    }

    public async Task<bool> CanAddInstallation(int tenantId)
    {
        var license = await GetActiveLicense(tenantId);
        if (license == null) return false;

        var currentCount = await _context.Installations
            .CountAsync(i => i.TenantId == tenantId && i.IsActive);
        return currentCount < license.MaxInstallations;
    }

    public async Task<bool> CanAddStockItemImage(int stockItemId)
    {
        var stockItem = await _context.StockItems.FindAsync(stockItemId);
        if (stockItem == null) return false;

        var license = await GetActiveLicense(stockItem.TenantId);
        if (license == null) return false;

        if (!license.AllowMultipleImages) return false;

        if (license.MaxImagesPerStockItem == null) return true; // Unlimited

        var currentCount = await _context.StockItemImages
            .CountAsync(i => i.StockItemId == stockItemId);
        return currentCount < license.MaxImagesPerStockItem;
    }
}

public class LicenseValidationResult
{
    public bool IsValid { get; set; }
    public string? Message { get; set; }
    public License? License { get; set; }
}
