namespace FitnessProfileService.Common.Responses
{
    public class RequestResponse: IRequestResponse
    {

        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public IDictionary<string, string[]>? Errors { get; set; }

        
        public static RequestResponse Ok(string message = "Success", int statusCode = 200)
        {
            return new RequestResponse
            {
                Success = true,
                StatusCode = statusCode,
                Message = message
            };
        }
        public static  RequestResponse Fail(string message, int statusCode = 400, IDictionary<string, string[]>? errors = null)
        {
            return new RequestResponse
            {
                Success = false,
                StatusCode = statusCode,
                Message = message,
                Errors = errors
            };
        }



    }

    public class RequestResponse <T>: RequestResponse
    {
        public T? Data { get; set; }

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
        public static RequestResponse<T> Created(T data, string message = "Created successfully")
        {
            return new RequestResponse<T>
            {
                Success = true,
                StatusCode = 201,
                Message = message,
                Data = data
            };
        }
        public static new RequestResponse<T> Fail(string message, int statusCode = 400, IDictionary<string, string[]>? errors = null)
        {
            return new RequestResponse<T>
            {
                Success = false,
                StatusCode = statusCode,
                Message = message,
                Data = default,
                Errors = errors
            };
        }
       
    }
}

