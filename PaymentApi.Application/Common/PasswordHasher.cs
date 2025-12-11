using System.Security.Cryptography;
using System.Text;

namespace PaymentApi.Application.Common
{
    public static class PasswordHasher
    {
        public static string Hash(string input)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}
