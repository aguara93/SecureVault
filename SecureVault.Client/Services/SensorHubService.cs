using Microsoft.AspNetCore.SignalR.Client;
using SecureVault.Shared.DTOs;

namespace SecureVault.Client.Services
{
    /// <summary>
    /// Manages the SignalR connection to the backend's sensor hub,
    /// allowing the Blazor client to receive real-time notifications
    /// whenever a new sensor reading is recorded.
    /// </summary>
    public class SensorHubService : IAsyncDisposable
    {
        private HubConnection? _hubConnection;

        /// <summary>
        /// Raised whenever a new sensor reading is received in real
        /// time from the backend via SignalR.
        /// </summary>
        public event Action<SensorReadingDto>? OnReadingReceived;

        /// <summary>
        /// Establishes a connection to the sensor hub at the specified
        /// API base URL and starts listening for incoming readings.
        /// </summary>
        /// <param name="apiBaseUrl">The base URL of the backend API
        /// hosting the SignalR hub.</param>
        /// <returns>A task that represents the asynchronous connection
        /// operation.</returns>
        public async Task StartAsync(string apiBaseUrl)
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl($"{apiBaseUrl}/hubs/sensor")
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<SensorReadingDto>("ReceiveSensorReading", (reading) =>
            {
                OnReadingReceived?.Invoke(reading);
            });

            await _hubConnection.StartAsync();
        }

        /// <summary>
        /// Disposes the underlying SignalR connection, if one has been
        /// established.
        /// </summary>
        /// <returns>A task that represents the asynchronous dispose
        /// operation.</returns>
        public async ValueTask DisposeAsync()
        {
            if (_hubConnection is not null)
            {
                await _hubConnection.DisposeAsync();
            }
        }
    }
}