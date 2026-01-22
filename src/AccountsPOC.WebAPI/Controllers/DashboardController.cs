using AccountsPOC.Domain.Entities;
using AccountsPOC.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountsPOC.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get overview statistics for management dashboard
    /// </summary>
    [HttpGet("overview")]
    public async Task<ActionResult<DashboardOverview>> GetOverview()
    {
        var today = DateTime.UtcNow.Date;

        var activeDrivers = await _context.Drivers
            .Where(d => d.IsActive)
            .CountAsync();

        var todayRoutes = await _context.DeliveryRoutes
            .Where(r => r.RouteDate.Date == today)
            .ToListAsync();

        var todayStops = await _context.DeliveryStops
            .Include(s => s.DeliveryRoute)
            .Where(s => s.DeliveryRoute.RouteDate.Date == today)
            .ToListAsync();

        var totalStops = todayStops.Count;
        var completedStops = todayStops.Count(s => s.Status == DeliveryStopStatus.Delivered);
        var inProgressStops = todayStops.Count(s => s.Status == DeliveryStopStatus.Arrived);
        var pendingStops = todayStops.Count(s => s.Status == DeliveryStopStatus.Pending);
        var failedStops = todayStops.Count(s => s.Status == DeliveryStopStatus.Failed);

        var scannedParcels = await _context.Parcels
            .Where(p => p.ScannedToVanAt != null && p.ScannedToVanAt.Value.Date == today)
            .CountAsync();

        var deliveredParcels = await _context.Parcels
            .Where(p => p.Status == "Delivered" && p.ScannedToVanAt != null && p.ScannedToVanAt.Value.Date == today)
            .CountAsync();

        return Ok(new DashboardOverview
        {
            ActiveDrivers = activeDrivers,
            TodayRoutes = todayRoutes.Count,
            ActiveRoutes = todayRoutes.Count(r => r.Status == "InProgress"),
            TotalStops = totalStops,
            CompletedStops = completedStops,
            InProgressStops = inProgressStops,
            PendingStops = pendingStops,
            FailedStops = failedStops,
            CompletionRate = totalStops > 0 ? Math.Round((double)completedStops / totalStops * 100, 1) : 0,
            ScannedParcels = scannedParcels,
            DeliveredParcels = deliveredParcels,
            LastUpdated = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Get active routes with real-time progress
    /// </summary>
    [HttpGet("active-routes")]
    public async Task<ActionResult<List<RouteProgress>>> GetActiveRoutes()
    {
        var today = DateTime.UtcNow.Date;

        var routes = await _context.DeliveryRoutes
            .Include(r => r.Driver)
            .Include(r => r.Stops)
            .Where(r => r.RouteDate.Date == today && r.Status != "Completed")
            .OrderBy(r => r.RouteDate)
            .ToListAsync();

        var routeProgress = routes.Select(r => new RouteProgress
        {
            RouteId = r.Id,
            RouteName = r.RouteName,
            DriverName = r.Driver?.FullName ?? "Unassigned",
            DriverPhone = r.Driver?.MobilePhone,
            Status = r.Status,
            TotalStops = r.Stops.Count,
            CompletedStops = r.Stops.Count(s => s.Status == DeliveryStopStatus.Delivered),
            InProgressStops = r.Stops.Count(s => s.Status == DeliveryStopStatus.Arrived),
            PendingStops = r.Stops.Count(s => s.Status == DeliveryStopStatus.Pending),
            FailedStops = r.Stops.Count(s => s.Status == DeliveryStopStatus.Failed),
            Progress = r.Stops.Count > 0 
                ? Math.Round((double)r.Stops.Count(s => s.Status == DeliveryStopStatus.Delivered) / r.Stops.Count * 100, 1) 
                : 0,
            StartTime = r.ActualStartTime,
            EstimatedCompletion = r.EstimatedEndTime,
            CurrentStopSequence = r.Stops
                .Where(s => s.Status == DeliveryStopStatus.Arrived)
                .Select(s => s.SequenceNumber)
                .FirstOrDefault()
        }).ToList();

        return Ok(routeProgress);
    }

    /// <summary>
    /// Get driver performance summary
    /// </summary>
    [HttpGet("driver-performance")]
    public async Task<ActionResult<List<DriverPerformance>>> GetDriverPerformance()
    {
        var today = DateTime.UtcNow.Date;
        var weekAgo = today.AddDays(-7);

        var drivers = await _context.Drivers
            .Include(d => d.DeliveryRoutes.Where(r => r.RouteDate >= weekAgo))
                .ThenInclude(r => r.Stops)
            .Where(d => d.IsActive)
            .ToListAsync();

        var performance = drivers.Select(d =>
        {
            var routes = d.DeliveryRoutes.ToList();
            var allStops = routes.SelectMany(r => r.Stops).ToList();
            var completedStops = allStops.Count(s => s.Status == DeliveryStopStatus.Delivered);
            var totalStops = allStops.Count;

            return new DriverPerformance
            {
                DriverId = d.Id,
                DriverName = d.FullName,
                Phone = d.MobilePhone ?? d.Phone,
                TotalRoutes = routes.Count,
                CompletedRoutes = routes.Count(r => r.Status == "Completed"),
                TotalDeliveries = totalStops,
                CompletedDeliveries = completedStops,
                FailedDeliveries = allStops.Count(s => s.Status == DeliveryStopStatus.Failed),
                SuccessRate = totalStops > 0 ? Math.Round((double)completedStops / totalStops * 100, 1) : 0,
                VehicleRegistration = d.VehicleRegistration,
                CurrentStatus = routes.Any(r => r.Status == "InProgress" && r.RouteDate.Date == today) 
                    ? "On Route" 
                    : "Available"
            };
        }).OrderByDescending(p => p.SuccessRate).ToList();

        return Ok(performance);
    }

    /// <summary>
    /// Get recent delivery activity (last 50 updates)
    /// </summary>
    [HttpGet("recent-activity")]
    public async Task<ActionResult<List<ActivityLog>>> GetRecentActivity()
    {
        var recentStops = await _context.DeliveryStops
            .Include(s => s.DeliveryRoute)
                .ThenInclude(r => r.Driver)
            .Include(s => s.Customer)
            .Where(s => s.DeliveryTime != null || s.Status != DeliveryStopStatus.Pending)
            .OrderByDescending(s => s.DeliveryTime ?? s.DeliveryRoute.RouteDate)
            .Take(50)
            .ToListAsync();

        var activities = recentStops.Select(s => new ActivityLog
        {
            Timestamp = s.DeliveryTime ?? DateTime.UtcNow,
            DriverName = s.DeliveryRoute?.Driver?.FullName ?? "Unknown",
            CustomerName = s.ContactName ?? s.Customer?.CompanyName ?? "Customer",
            Address = s.DeliveryAddress,
            Status = s.Status,
            Action = GetActionDescription(s.Status)
        }).ToList();

        return Ok(activities);
    }

    private static string GetActionDescription(string status)
    {
        return status switch
        {
            "Completed" => "✅ Delivery completed",
            "Failed" => "❌ Delivery failed",
            "InProgress" => "🚚 En route to customer",
            _ => "📦 Delivery pending"
        };
    }
}

public record DashboardOverview
{
    public int ActiveDrivers { get; set; }
    public int TodayRoutes { get; set; }
    public int ActiveRoutes { get; set; }
    public int TotalStops { get; set; }
    public int CompletedStops { get; set; }
    public int InProgressStops { get; set; }
    public int PendingStops { get; set; }
    public int FailedStops { get; set; }
    public double CompletionRate { get; set; }
    public int ScannedParcels { get; set; }
    public int DeliveredParcels { get; set; }
    public DateTime LastUpdated { get; set; }
}

public record RouteProgress
{
    public int RouteId { get; set; }
    public string RouteName { get; set; } = string.Empty;
    public string DriverName { get; set; } = string.Empty;
    public string? DriverPhone { get; set; }
    public string Status { get; set; } = string.Empty;
    public int TotalStops { get; set; }
    public int CompletedStops { get; set; }
    public int InProgressStops { get; set; }
    public int PendingStops { get; set; }
    public int FailedStops { get; set; }
    public double Progress { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EstimatedCompletion { get; set; }
    public int CurrentStopSequence { get; set; }
}

public record DriverPerformance
{
    public int DriverId { get; set; }
    public string DriverName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public int TotalRoutes { get; set; }
    public int CompletedRoutes { get; set; }
    public int TotalDeliveries { get; set; }
    public int CompletedDeliveries { get; set; }
    public int FailedDeliveries { get; set; }
    public double SuccessRate { get; set; }
    public string? VehicleRegistration { get; set; }
    public string CurrentStatus { get; set; } = string.Empty;
}

public record ActivityLog
{
    public DateTime Timestamp { get; set; }
    public string DriverName { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
}
