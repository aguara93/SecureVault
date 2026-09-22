using System.Security.Cryptography;

namespace SecureVault.API.Services
{
    public class ApiKeyService
    {
        /// <summary>
        /// Generates a new cryptographically secure random API key
        /// encoded as a Base64 string.
        /// </summary>
        /// <returns>A newly generated API key.</returns>
        // Generate a new random Api key
        public string GenerateApiKey()
        {
            var bytes = RandomNumberGenerator.GetBytes(32);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Hashes a raw API key using BCrypt, so that only the hash
        /// needs to be stored in the database rather than the key
        /// itself.
        /// </summary>
        /// <param name="apiKey">The raw API key to hash.</param>
        /// <returns>The hashed representation of the API key.</returns>
        // Hash an API key for secure storage
        public string HashApiKey(string apiKey)
        {
            return BCrypt.Net.BCrypt.HashPassword(apiKey);
        }

        /// <summary>
        /// Verifies a raw API key against a previously stored hash.
        /// </summary>
        /// <param name="apiKey">The raw API key provided by the caller.</param>
        /// <param name="hash">The stored hash to verify against.</param>
        /// <returns><c>true</c> if the API key matches the hash;
        /// otherwise, <c>false</c>.</returns>
        // Verify a given API key against the stored hash
        public bool VerifyApiKey(string apiKey, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(apiKey, hash);
        }
    }
}
