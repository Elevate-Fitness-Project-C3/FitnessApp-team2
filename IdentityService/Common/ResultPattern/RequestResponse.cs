using System.Text.Json.Serialization;

namespace IdentityService.Common.ResultPattern;

public class RequestResponse<T>
{
    public bool Success { get; set; }

    public int StatusCode { get; set; }

    public T? Data { get; set; }

    public string Message { get; set; } = string.Empty;

    public string? ErrorCode { get; set; }

    public IDictionary<string, string[]>? Errors { get; set; }


    public static RequestResponse<T> Ok(T data, string message = "Success", int statusCode = 200)
    {
        return new RequestResponse<T>
        {
            Success = true,
            StatusCode = statusCode,
            Message = message,
            Data = data
        };
    }

    public static RequestResponse<T> Fail(
        string message, int statusCode = 400, IDictionary<string, string[]>? errors = null,string? errorCode = null)
    {
        return new RequestResponse<T>
        {
            Success = false,
            StatusCode = statusCode,
            Message = message,
            ErrorCode = errorCode,
            Errors = errors
        };
    }
}

public class RequestResponse : RequestResponse<object>
{
    public static RequestResponse Ok(string message = "Success", int statusCode = 200)
    {
        return new RequestResponse
        {
            Success = true,
            StatusCode = statusCode,
            Message = message
        };
    }

    public static new RequestResponse Fail(
        string message, int statusCode = 400, IDictionary<string, string[]>? errors = null,
        string? errorCode = null)
    {
        return new RequestResponse
        {
            Success = false,
            StatusCode = statusCode,
            Message = message,
            ErrorCode = errorCode,
            Errors = errors
        };
    }
}
