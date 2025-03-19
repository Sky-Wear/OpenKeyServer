namespace OpenKeyServer.Data.Request;

public class SetKeyV3
{
    public string aesKey { get; set; }
    public string keyId { get; set; }
    public string eebbkKey { get; set; }
    public string bindNumber { get; set; }
    public string chipId { get; set; }
}