using System.Net.Http.Json;
using SecureVault.Shared.DTOs;

namespace SecureVault.Client.Services
{
    public class AlarmService
    {
        private readonly HttpClient _http;

        public AlarmService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<AlarmEventDto>> GetAlarmEventsAsync()
        {
            return await _http.GetFromJsonAsync<List<AlarmEventDto>>("api/alarmevents") 
                ?? new List<AlarmEventDto>();
        }

        public async Task<List<AlarmEventDto>> GetActiveAlarmsAsync()
        {
            return await _http.GetFromJsonAsync<List<AlarmEventDto>>("api/alarmevents/active") 
                ?? new List<AlarmEventDto>();
        }
    }
}   