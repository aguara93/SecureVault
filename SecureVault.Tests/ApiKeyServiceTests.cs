using SecureVault.API.Services;
using Xunit;

namespace SecureVault.Tests
{
    /// <summary>
    /// Unit tests for <see cref="ApiKeyService"/>, verifying that API
    /// keys are generated, hashed, and verified correctly.
    /// </summary>
    public class ApiKeyServiceTests
    {
        private readonly ApiKeyService _service = new();

        // Verifies that a generated API key is not null or empty
        [Fact]
        public void GenerateApiKey_ReturnsNonEmptyString()
        {
            var key = _service.GenerateApiKey();

            Assert.False(string.IsNullOrEmpty(key));
        }

        // Verifies that two generated API keys are unique
        [Fact]
        public void GenerateApiKey_CalledTwice_ReturnsDifferentKeys()
        {
            var key1 = _service.GenerateApiKey();
            var key2 = _service.GenerateApiKey();

            Assert.NotEqual(key1, key2);
        }

        // Verifies that a correctly hashed API key can be successfully verified
        [Fact]
        public void VerifyApiKey_CorrectKey_ReturnsTrue()
        {
            var key = _service.GenerateApiKey();
            var hash = _service.HashApiKey(key);

            var result = _service.VerifyApiKey(key, hash);

            Assert.True(result);
        }

        // Verifies that an incorrect API key fails verification against a stored hash
        [Fact]
        public void VerifyApiKey_IncorrectKey_ReturnsFalse()
        {
            var key = _service.GenerateApiKey();
            var hash = _service.HashApiKey(key);
            var wrongKey = _service.GenerateApiKey();

            var result = _service.VerifyApiKey(wrongKey, hash);

            Assert.False(result);
        }
    }
}