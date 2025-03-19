namespace OpenKeyServer.Data.Response;

public class CommonResponse
{
    private int _code;
    public string msg { get; set; } = "unknown";

    public int code
    {
        get => _code;
        set
        {
            msg= CodeHelper.GetMsg((Code)value);
            _code = value;
        }
    }
    public object? data { get; set; }
}