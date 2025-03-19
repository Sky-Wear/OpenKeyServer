namespace OpenKeyServer.Data.Request;

public class SetKeyV2
{
    public string rsaPublicKey { get; set; }
    public string keyId { get; set; }
    public string bindNumber { get; set; }
    public string chipId { get; set; }
}