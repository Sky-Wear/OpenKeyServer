namespace OpenKeyServer;

public interface IKeyValidation
{
    public bool ValidateKey(string key, string bindNumber,string chipId);
}