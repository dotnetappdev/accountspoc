namespace AccountsPOC.WebAPI.Models;

public record UpdateContactRequest(string? ContactName, string? ContactPhone, string? ContactEmail);
public record CaptureEvidenceRequest(string? SignatureImagePath, string? PhotoEvidencePaths);
