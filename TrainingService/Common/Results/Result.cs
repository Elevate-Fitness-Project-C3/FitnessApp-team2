namespace TrainingService.Common.Results;

public enum ResultStatus
{
    Ok,
    NotFound,
    Conflict,
    BadRequest
}

public class Result<T>
{
    public bool IsSuccess => Status == ResultStatus.Ok;
    public bool IsNotFound => Status == ResultStatus.NotFound;
    public bool IsConflict => Status == ResultStatus.Conflict;

    public ResultStatus Status { get; private set; }
    public T? Value { get; private set; }
    public string? ErrorMessage { get; private set; }

    private Result() { }

    public static Result<T> Success(T value) =>
        new()
        {
            Status = ResultStatus.Ok,
            Value = value
        };

    public static Result<T> NotFound(string errorMessage) =>
        new()
        {
            Status = ResultStatus.NotFound,
            ErrorMessage = errorMessage
        };

    public static Result<T> Conflict(string errorMessage) =>
        new()
        {
            Status = ResultStatus.Conflict,
            ErrorMessage = errorMessage
        };

    public static Result<T> BadRequest(string errorMessage) =>
        new()
        {
            Status = ResultStatus.BadRequest,
            ErrorMessage = errorMessage
        };
}

public class Result
{
    public bool IsSuccess => Status == ResultStatus.Ok;
    public bool IsNotFound => Status == ResultStatus.NotFound;
    public bool IsConflict => Status == ResultStatus.Conflict;

    public ResultStatus Status { get; private set; }
    public string? ErrorMessage { get; private set; }

    private Result() { }

    public static Result Success() =>
        new()
        {
            Status = ResultStatus.Ok
        };

    public static Result NotFound(string errorMessage) =>
        new()
        {
            Status = ResultStatus.NotFound,
            ErrorMessage = errorMessage
        };

    public static Result Conflict(string errorMessage) =>
        new()
        {
            Status = ResultStatus.Conflict,
            ErrorMessage = errorMessage
        };

    public static Result BadRequest(string errorMessage) =>
        new()
        {
            Status = ResultStatus.BadRequest,
            ErrorMessage = errorMessage
        };
}
