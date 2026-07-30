using System.Net.Http.Json;
using System.Text.Json;
using SecureVault.Shared.DTOs;

namespace SecureVault.Client.Services
{
    public class AuthService
    {
        private readonly HttpClient _http;
        private string? _token;

        public event Action? OnAuthStateChanged;

        public AuthService(HttpClient http)
        {
            _http = http;
        }

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

        public bool IsLoggedIn => _token != null;

        public void Logout()
        {
            _token = null;
            _http.DefaultRequestHeaders.Authorization = null;
            OnAuthStateChanged?.Invoke();
        }
    }
}
