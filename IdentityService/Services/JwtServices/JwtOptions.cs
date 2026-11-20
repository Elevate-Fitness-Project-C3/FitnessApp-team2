namespace IdentityService.Services.JwtServices
{
    public class JwtOptions
    {
        public string Issure { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int DutrationInMinute { get; set; }
        public string SecretKey { get; set; } = string.Empty;
    }
}
