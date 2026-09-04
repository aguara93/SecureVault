using Microsoft.AspNetCore.SignalR.Client;
using SecureVault.Shared.DTOs;

namespace SecureVault.Client.Services
{
    public class SensorHubService : IAsyncDisposable
    {
        private HubConnection? _hubConnection;

        public event Action<SensorReadingDto>? OnReadingReceived;

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

        public async ValueTask DisposeAsync()
        {
            if (_hubConnection is not null)
            {
                await _hubConnection.DisposeAsync();
            }
        }
    }
}