namespace OpenKeyServer.Data.Response;

public class GetKeyResponse
{
    public string Key { get; set; }
    public int Type { get; set; }
    public long LastUpdated { get; set; }
}