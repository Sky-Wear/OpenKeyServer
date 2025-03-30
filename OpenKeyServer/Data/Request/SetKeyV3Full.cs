namespace OpenKeyServer.Data.Request;

public class SetKeyV3Full
{
    public string fullAesKey { get; set; }
    public string keyId { get; set; }
    public string rsaPublicKey { get; set; }
    public string bindNumber { get; set; }
    public string chipId { get; set; }
}