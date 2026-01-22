using AccountsPOC.MauiApp.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Net.Http.Json;

namespace AccountsPOC.MauiApp.Services;

public class SyncService : ISyncService
{
    private readonly LocalDbContext _localDb;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConnectivity _connectivity;

    public SyncService(LocalDbContext localDb, IHttpClientFactory httpClientFactory, IConnectivity connectivity)
    {
        _localDb = localDb;
        _httpClientFactory = httpClientFactory;
        _connectivity = connectivity;
    }

    public async Task<bool> IsOnlineAsync()
    {
        return _connectivity.NetworkAccess == NetworkAccess.Internet;
    }

    public async Task<SyncStatus> GetSyncStatusAsync()
    {
        var pendingChanges = await _localDb.SyncLogs
            .Where(log => !log.Synced)
            .CountAsync();

        var lastSync = await _localDb.SyncLogs
            .Where(log => log.Synced)
            .OrderByDescending(log => log.SyncedAt)
            .Select(log => log.SyncedAt)
            .FirstOrDefaultAsync();

        return new SyncStatus
        {
            LastSyncTime = lastSync,
            PendingChanges = pendingChanges,
            IsOnline = await IsOnlineAsync()
        };
    }

    public async Task<bool> SyncDataAsync()
    {
        if (!await IsOnlineAsync())
        {
            return false;
        }

        try
        {
            // First download new data from server
            await DownloadDataAsync();

            // Then upload local changes
            await UploadChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            // Log error
            Console.WriteLine($"Sync error: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DownloadDataAsync()
    {
        if (!await IsOnlineAsync())
        {
            return false;
        }

        var httpClient = _httpClientFactory.CreateClient("AccountsPOCAPI");

        try
        {
            // Download delivery routes
            var routes = await httpClient.GetFromJsonAsync<List<AccountsPOC.Domain.Entities.DeliveryRoute>>("api/deliveryroutes");
            if (routes != null)
            {
                foreach (var route in routes)
                {
                    var existing = await _localDb.DeliveryRoutes.FindAsync(route.Id);
                    if (existing == null)
                    {
                        _localDb.DeliveryRoutes.Add(route);
                    }
                    else
                    {
                        _localDb.Entry(existing).CurrentValues.SetValues(route);
                    }
                }
            }

            // Download drivers
            var drivers = await httpClient.GetFromJsonAsync<List<AccountsPOC.Domain.Entities.Driver>>("api/drivers");
            if (drivers != null)
            {
                foreach (var driver in drivers)
                {
                    var existing = await _localDb.Drivers.FindAsync(driver.Id);
                    if (existing == null)
                    {
                        _localDb.Drivers.Add(driver);
                    }
                    else
                    {
                        _localDb.Entry(existing).CurrentValues.SetValues(driver);
                    }
                }
            }

            // Download stock items
            var stockItems = await httpClient.GetFromJsonAsync<List<AccountsPOC.Domain.Entities.StockItem>>("api/stockitems");
            if (stockItems != null)
            {
                foreach (var item in stockItems)
                {
                    var existing = await _localDb.StockItems.FindAsync(item.Id);
                    if (existing == null)
                    {
                        _localDb.StockItems.Add(item);
                    }
                    else
                    {
                        _localDb.Entry(existing).CurrentValues.SetValues(item);
                    }
                }
            }

            await _localDb.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Download error: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> UploadChangesAsync()
    {
        if (!await IsOnlineAsync())
        {
            return false;
        }

        var httpClient = _httpClientFactory.CreateClient("AccountsPOCAPI");
        var pendingLogs = await _localDb.SyncLogs
            .Where(log => !log.Synced)
            .OrderBy(log => log.Timestamp)
            .ToListAsync();

        foreach (var log in pendingLogs)
        {
            try
            {
                // Upload based on entity type and operation
                if (log.EntityType == "DeliveryStop" && !string.IsNullOrEmpty(log.DataJson))
                {
                    var entity = JsonSerializer.Deserialize<AccountsPOC.Domain.Entities.DeliveryStop>(log.DataJson);
                    if (entity != null)
                    {
                        switch (log.Operation)
                        {
                            case "Update":
                                await httpClient.PutAsJsonAsync($"api/deliveryroutes/stops/{entity.Id}", entity);
                                break;
                        }
                    }
                }

                // Mark as synced
                log.Synced = true;
                log.SyncedAt = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                log.SyncError = ex.Message;
            }
        }

        await _localDb.SaveChangesAsync();
        return true;
    }
}
