namespace FitnessProfileService.Common.Responses
{
   
        public static class RequestResponseFactory
        {
            public static TResponse ValidationFailure<TResponse>(IDictionary<string, string[]> errors)
            {
                var response = (TResponse)Activator.CreateInstance(typeof(TResponse))!;

                var baseResponse = (IRequestResponse)response!;

                baseResponse.Success = false;
                baseResponse.StatusCode = 400;
                baseResponse.Message = "Validation failed.";
                baseResponse.Errors = errors;

                return response;
            }
        
    }
}
