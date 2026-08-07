using System.Net.Http.Json;
using SecureVault.Shared.DTOs;

namespace SecureVault.Client.Services
{
    public class SensorService
    {
        private readonly HttpClient _http;

        public SensorService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<SensorDto>> GetSensorsAsync()
        {
            return await _http.GetFromJsonAsync<List<SensorDto>>("api/sensors") 
                ?? new List<SensorDto>();
        }

        public async Task<bool> CreateSensorAsync(SensorDto sensor)
        {
            var response = await _http.PostAsJsonAsync("api/sensors", sensor);
            return response.IsSuccessStatusCode;
        }
    }
}
