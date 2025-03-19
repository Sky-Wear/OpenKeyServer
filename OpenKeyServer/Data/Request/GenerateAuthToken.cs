namespace OpenKeyServer.Data.Request;

public class GenerateAuthToken
{
    public string? apiSecret { get; set; }
    public string? keyId { get; set; }
}