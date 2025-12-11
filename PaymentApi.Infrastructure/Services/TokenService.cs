using PaymentApi.Application.Interfaces.Services;
using System.Security.Cryptography;

namespace PaymentApi.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        public string GenerateToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(64);
            var base64 = Convert.ToBase64String(bytes);
            return base64.Replace("+", "-").Replace("/", "_").TrimEnd('=');
        }
    }
}
