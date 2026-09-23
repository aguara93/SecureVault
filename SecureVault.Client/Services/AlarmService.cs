using System.Net.Http.Json;
using SecureVault.Shared.DTOs;

namespace SecureVault.Client.Services
{
    /// <summary>
    /// Provides methods for retrieving alarm events from the backend
    /// API, including both the full alarm history and only the
    /// currently active alarms.
    /// </summary>
    public class AlarmService
    {
        private readonly HttpClient _http;

        public AlarmService(HttpClient http)
        {
            _http = http;
        }

        /// <summary>
        /// Retrieves all alarm events, including both active and
        /// resolved alarms.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains a list of all alarm events, or an
        /// empty list if none exist.</returns>
        public async Task<List<AlarmEventDto>> GetAlarmEventsAsync()
        {
            return await _http.GetFromJsonAsync<List<AlarmEventDto>>("api/alarmevents") 
                ?? new List<AlarmEventDto>();
        }

        /// <summary>
        /// Retrieves only the alarm events that are currently active,
        /// i.e. that have not yet been resolved.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains a list of active alarm events, or
        /// an empty list if none exist.</returns>
        public async Task<List<AlarmEventDto>> GetActiveAlarmsAsync()
        {
            return await _http.GetFromJsonAsync<List<AlarmEventDto>>("api/alarmevents/active") 
                ?? new List<AlarmEventDto>();
        }
    }
}   