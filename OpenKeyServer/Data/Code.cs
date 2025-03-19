namespace OpenKeyServer.Data;

public enum Code
{
    Success = 200,
    InternalError = 10000,
    AuthFailed = 1000,
    InvalidRequest = 1001
}

public static class CodeHelper
{
    public static string GetMsg(this Code code)
    {
        switch (code)
        {
            case Code.Success:
            {
                return "success";
            }
            case Code.InternalError:
            {
                return "server internal error";
            }
            case Code.AuthFailed:
            case Code.InvalidRequest:
            default:
            {
                return "you know what's wrong. lol.";
            }
        }
    }
}