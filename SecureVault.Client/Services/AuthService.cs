using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.JSInterop;
using SecureVault.Shared.DTOs;

namespace SecureVault.Client.Services
{
    public class AuthService
    {
        private readonly HttpClient _http;
        private readonly IJSRuntime _js;
        private string? _token;

        public AuthService(HttpClient http, IJSRuntime js)
        {
            _http = http;
            _js = js;
        }

        public async Task<bool> LoginAsync(LoginDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/auth/login", dto);
            if (response.IsSuccessStatusCode)
                return false;

            var result = await response.Content.ReadFromJsonAsync<JsonElement>();
            _token = result.GetProperty("token").GetString();

            // Save token in memory
            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

            return true;
        }

        public bool IsLoggedIn() => _token != null;

        public void Logout()
        {
            _token = null;
            _http.DefaultRequestHeaders.Authorization = null;
        }
    }
}
