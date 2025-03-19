namespace OpenKeyServer.Data.Request;

public class GetKeyRequest
{
    public string? apiSecret { get; set; }  
    public string? keyId { get; set; }
}