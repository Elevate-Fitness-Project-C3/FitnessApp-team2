using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using static IdentityService.Services.JwtServices.ITokenService;

namespace IdentityService.Services.JwtServices
{
    public class TokenService : ITokenService
    {
        private readonly JwtOptions _options;

        public TokenService(JwtOptions options)
        {
            _options = options;
        }
        public TokenResult GenerateTokens(string userId, string email, IEnumerable<string> roles, Guid? Trainer)
        {
            // expirationss 
            var accessTokenExpiresAt = DateTime.UtcNow.AddMinutes(_options.DutrationInMinute);
            // claims 
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Email,email),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()), // توقيع خاص للتوكين 
              
            };

            foreach(var role in roles)
            {
                claims.Add(new Claim("roles", role));
            }

            // 3. تجهيز المفتاح وطريقة توقيع الـ JWT
            var signingCredentials = new SigningCredentials
              (new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)), SecurityAlgorithms.HmacSha256);

            // 4. إنشاء الـ JWT
            var jwt = new JwtSecurityToken(
                issuer: _options.Issure,
                audience: _options.Audience,
                claims: claims,
                notBefore: DateTimeOffset.Now.UtcDateTime,
                expires: accessTokenExpiresAt,
                signingCredentials: signingCredentials);

            // 5. تحويل الـ JWT إلى الكود الذي سيُرسل للعميل
            var tokenHandler = new JwtSecurityTokenHandler().WriteToken(jwt);

            return new TokenResult(AccessToken: tokenHandler, ExpiresIn: accessTokenExpiresAt);
        }
    }
}
