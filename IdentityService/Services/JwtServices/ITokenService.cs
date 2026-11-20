namespace IdentityService.Services.JwtServices
{
    public interface ITokenService
    {
        TokenResult GenerateTokens(string userId, string email, IEnumerable<string> roles);

        public record TokenResult(string AccessToken, DateTimeOffset ExpiresIn);
       

    }
}
