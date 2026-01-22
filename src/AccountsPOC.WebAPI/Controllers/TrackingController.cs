using AccountsPOC.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountsPOC.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TrackingController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TrackingController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get delivery status by tracking number (DeliveryStop ID or Parcel Barcode)
    /// </summary>
    [HttpGet("delivery/{trackingNumber}")]
    public async Task<ActionResult<DeliveryTrackingInfo>> GetDeliveryStatus(string trackingNumber)
    {
        // Try to find by delivery stop ID
        if (int.TryParse(trackingNumber, out var stopId))
        {
            var stop = await _context.DeliveryStops
                .Include(s => s.DeliveryRoute)
                    .ThenInclude(r => r.Driver)
                .Include(s => s.Customer)
                .FirstOrDefaultAsync(s => s.Id == stopId);

            if (stop != null)
            {
                return Ok(MapToTrackingInfo(stop));
            }
        }

        // Try to find by parcel barcode
        var parcel = await _context.Parcels
            .Include(p => p.DeliveryStop)
                .ThenInclude(s => s.DeliveryRoute)
                    .ThenInclude(r => r.Driver)
            .Include(p => p.DeliveryStop.Customer)
            .FirstOrDefaultAsync(p => p.ParcelBarcode == trackingNumber);

        if (parcel != null && parcel.DeliveryStop != null)
        {
            var trackingInfo = MapToTrackingInfo(parcel.DeliveryStop);
            trackingInfo.ParcelBarcode = parcel.ParcelBarcode;
            trackingInfo.ParcelStatus = parcel.Status;
            return Ok(trackingInfo);
        }

        return NotFound(new { message = "Tracking number not found" });
    }

    /// <summary>
    /// Get all deliveries for a customer by email
    /// </summary>
    [HttpGet("customer/{email}")]
    public async Task<ActionResult<List<DeliveryTrackingInfo>>> GetCustomerDeliveries(string email)
    {
        var stops = await _context.DeliveryStops
            .Include(s => s.DeliveryRoute)
                .ThenInclude(r => r.Driver)
            .Include(s => s.Customer)
            .Where(s => s.Customer != null && s.Customer.Email == email)
            .OrderByDescending(s => s.DeliveryRoute.RouteDate)
            .Take(20)
            .ToListAsync();

        return Ok(stops.Select(MapToTrackingInfo).ToList());
    }

    private static DeliveryTrackingInfo MapToTrackingInfo(AccountsPOC.Domain.Entities.DeliveryStop stop)
    {
        return new DeliveryTrackingInfo
        {
            TrackingNumber = stop.Id.ToString(),
            Status = stop.Status,
            CustomerName = stop.ContactName ?? stop.Customer?.CompanyName ?? "Customer",
            DeliveryAddress = stop.DeliveryAddress,
            EstimatedDeliveryTime = stop.ArrivalTime,
            ActualDeliveryTime = stop.DeliveryTime,
            DriverName = stop.DeliveryRoute?.Driver?.FullName,
            DriverPhone = stop.DeliveryRoute?.Driver?.MobilePhone,
            RouteDate = stop.DeliveryRoute?.RouteDate,
            SequenceNumber = stop.SequenceNumber,
            SafePlace = stop.SafePlace,
            DeliveryInstructions = stop.DeliveryNotes,
            SignatureImagePath = stop.SignatureImagePath,
            PhotoEvidencePath = stop.PhotoEvidencePaths,
            Latitude = (decimal?)stop.Latitude,
            Longitude = (decimal?)stop.Longitude,
            OtpVerified = stop.OTPVerified
        };
    }
}

public record DeliveryTrackingInfo
{
    public string TrackingNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string DeliveryAddress { get; set; } = string.Empty;
    public DateTime? EstimatedDeliveryTime { get; set; }
    public DateTime? ActualDeliveryTime { get; set; }
    public string? DriverName { get; set; }
    public string? DriverPhone { get; set; }
    public DateTime? RouteDate { get; set; }
    public int SequenceNumber { get; set; }
    public string? SafePlace { get; set; }
    public string? DeliveryInstructions { get; set; }
    public string? SignatureImagePath { get; set; }
    public string? PhotoEvidencePath { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public bool OtpVerified { get; set; }
    public string? ParcelBarcode { get; set; }
    public string? ParcelStatus { get; set; }
}
