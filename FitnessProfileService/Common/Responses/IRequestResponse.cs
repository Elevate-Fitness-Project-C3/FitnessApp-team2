namespace FitnessProfileService.Common.Responses
{
    public interface IRequestResponse
    {
        bool Success { get; set; }
        int StatusCode { get; set; }
        string Message { get; set; }
        IDictionary<string, string[]>? Errors { get; set; }
    }
}
