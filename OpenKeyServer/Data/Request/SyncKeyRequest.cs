namespace OpenKeyServer.Data.Request;

public class SyncKeyRequest
{
    public string? apiSecret { get; set; }
    public long? fromTimestamp { get; set; } = -1;
}