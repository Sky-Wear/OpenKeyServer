namespace OpenKeyServer.Data.Response;

public class KeyItemResponse
{
    public string Id { get; set; }
    public GetKeyResponse Value { get; set; }
}

public class SyncKeyResponse
{
    public List<KeyItemResponse> Items { get; set; }
}