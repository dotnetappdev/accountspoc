using AccountsPOC.Domain.Entities;

namespace AccountsPOC.WebAPI.Models;

public record UpdateContactRequest(string? ContactName, string? ContactPhone, string? ContactEmail);
public record CaptureEvidenceRequest(string? SignatureImagePath, string? PhotoEvidencePaths);
public record VerifyOTPRequest(string OTPCode);
public record UpdateSafePlaceRequest(string? SafePlace, string? DoorAccessCode, string? PostBoxCode, string? BuildingAccessInstructions);
public record ReorderStopsRequest(List<int> StopIds);
public record UpdateDeliveryStatusRequest(DeliveryStatusType DeliveryStatus, string? NeighborDoorNumber, string? DeliveryNotes);
