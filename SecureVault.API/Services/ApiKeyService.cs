using System.Security.Cryptography;

namespace SecureVault.API.Services
{
    public class ApiKeyService
    {
        // Generate a new random Api key
        public string GenerateApiKey()
        {
            var bytes = RandomNumberGenerator.GetBytes(32);
            return Convert.ToBase64String(bytes);
        }

        // Hash an API key for secure storage
        public string HashApiKey(string apiKey)
        {
            return BCrypt.Net.BCrypt.HashPassword(apiKey);
        }

        // Verify a given API key against the stored hash
        public bool VerifyApiKey(string apiKey, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(apiKey, hash);
        }
    }
}
