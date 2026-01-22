namespace AccountsPOC.MauiApp.Services;

public interface ISyncService
{
    Task<bool> SyncDataAsync();
    Task<bool> IsOnlineAsync();
    Task<SyncStatus> GetSyncStatusAsync();
    Task<bool> DownloadDataAsync();
    Task<bool> UploadChangesAsync();
}

public class SyncStatus
{
    public DateTime? LastSyncTime { get; set; }
    public int PendingChanges { get; set; }
    public bool IsOnline { get; set; }
    public string? LastError { get; set; }
}
