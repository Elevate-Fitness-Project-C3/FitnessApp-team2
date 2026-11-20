namespace IdentityService.Services.JwtServices
{
    public class JwtOptions
    {
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int DurationInMinutes { get; set; }
        public int RefreshDurationInDays { get; set; } = 7;
        public string SecretKey { get; set; } = string.Empty;
    }
}
