using System.Net.Http.Json;
using System.Text.Json;
using SecureVault.Shared.DTOs;

namespace SecureVault.Client.Services
{
    /// <summary>
    /// Manages user authentication in the Blazor client, including
    /// logging in, tracking login state, and attaching the JWT to
    /// outgoing HTTP requests. The token is kept in memory only and
    /// is lost on page reload.
    /// </summary>
    public class AuthService
    {
        private readonly HttpClient _http;
        private string? _token;

        /// <summary>
        /// Raised whenever the authentication state changes, i.e.
        /// after a successful login or logout, so that other
        /// components can react accordingly.
        /// </summary>
        public event Action? OnAuthStateChanged;

        public AuthService(HttpClient http)
        {
            _http = http;
        }

        /// <summary>
        /// Attempts to log in with the provided credentials. On
        /// success, the returned JWT is stored in memory and attached
        /// as a Bearer token to the HttpClient for subsequent requests.
        /// </summary>
        /// <param name="dto">The login credentials to authenticate with.</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result is <c>true</c> if the login succeeded;
        /// otherwise, <c>false</c>.</returns>
        public async Task<bool> LoginAsync(LoginDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/auth/login", dto);
            if (!response.IsSuccessStatusCode)
                return false;

            var result = await response.Content.ReadFromJsonAsync<JsonElement>();
            _token = result.GetProperty("token").GetString();

            // Save token in memory
            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

            return true;
        }

        /// <summary>
        /// Gets a value indicating whether a user is currently
        /// authenticated, based on whether a token is held in memory.
        /// </summary>
        public bool IsLoggedIn => _token != null;

        /// <summary>
        /// Logs out the current user by clearing the stored token and
        /// removing the Authorization header from the HttpClient.
        /// </summary>
        public void Logout()
        {
            _token = null;
            _http.DefaultRequestHeaders.Authorization = null;
            OnAuthStateChanged?.Invoke();
        }
    }
}
