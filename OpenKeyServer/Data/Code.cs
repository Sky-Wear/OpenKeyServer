namespace OpenKeyServer.Data;

public enum Code
{
    Success = 200,
    InternalError = 10000,
    AuthFailed = 1000,
    InvalidRequest = 1001, 
    NoSuchSource = 1002, 
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
            case Code.NoSuchSource:
            {
                return "no such source";
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