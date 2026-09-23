using System.Net.Http.Json;
using SecureVault.Shared.DTOs;

namespace SecureVault.Client.Services
{
    /// <summary>
    /// Provides methods for retrieving, creating and deleting sensors,
    /// as well as fetching their reading history, by communicating
    /// with backend API.
    /// </summary>
    public class SensorService
    {
        private readonly HttpClient _http;

        public SensorService(HttpClient http)
        {
            _http = http;
        }

        /// <summary>
        /// Retrieves all sensors registered in the system.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains a list of all sensors, or an empty
        /// list if none exist or the request fails to deserialize.</returns>
        public async Task<List<SensorDto>> GetSensorsAsync()
        {
            return await _http.GetFromJsonAsync<List<SensorDto>>("api/sensors") 
                ?? new List<SensorDto>();
        }

        /// <summary>
        /// Creates a new sensor by sending its details to the backend API.
        /// </summary>
        /// <param name="sensor">The sensor data to create.</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result is <c>true</c> if the sensor was created
        /// successfully; otherwise, <c>false</c>.</returns>
        public async Task<bool> CreateSensorAsync(SensorDto sensor)
        {
            var response = await _http.PostAsJsonAsync("api/sensors", sensor);
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Retrieves the reading history for a specific sensor.
        /// </summary>
        /// <param name="sensorId">The unique identifier of the sensor
        /// whose readings should be retrieved.</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains a list of readings for the
        /// specified sensor, or an empty list if none exist.</returns>
        public async Task<List<SensorReadingDto>> GetSensorReadingsAsync(int sensorId)
        {
            return await _http.GetFromJsonAsync<List<SensorReadingDto>>($"api/sensorreadings/sensor/{sensorId}") 
                ?? new List<SensorReadingDto>();
        }

        /// <summary>
        /// Deletes a sensor identified by its unique ID.
        /// </summary>
        /// <param name="id">The unique identifier of the sensor to delete.</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result is <c>true</c> if the deletion succeeded;
        /// otherwise, <c>false</c>.</returns>
        public async Task<bool> DeleteSensorAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/sensors/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
