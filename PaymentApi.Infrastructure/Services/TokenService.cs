using Microsoft.Extensions.Configuration;
using PaymentApi.Application.Interfaces.Services;
using PaymentApi.Domain.Entities;
using System.Security.Cryptography;

namespace PaymentApi.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        public string GenerateToken()
        {
            // Генерируем 64 байта и кодируем Base64Url (убираем = и + /)
            var bytes = RandomNumberGenerator.GetBytes(64);
            var base64 = Convert.ToBase64String(bytes);
            // Простейшая нормализация (можно лучше, но достаточно для токена)
            return base64.Replace("+", "-").Replace("/", "_").TrimEnd('=');
        }
    }
}
