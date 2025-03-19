namespace OpenKeyServer.Data.Request;

public class GenerateCodeRequest
{
    public string? apiSecret { get; set; }
    public string? keyId { get; set; }
}