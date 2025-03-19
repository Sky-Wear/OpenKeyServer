namespace OpenKeyServer;

public class DemoValidation : IKeyValidation
{
    public bool ValidateKey(string key, string bindNumber, string chipId)
    {
        return true;
    }
}