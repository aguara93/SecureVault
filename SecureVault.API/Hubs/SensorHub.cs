using Microsoft.AspNetCore.SignalR;

namespace SecureVault.API.Hubs
{
    /// <summary>
    /// SignalR hub used to push real-time notifications to connected
    /// clients whenever a new sensor reading is recorded. This hub
    /// currently defines no server-side methods; it is used solely as
    /// a broadcast channel from <see cref="Controllers.SensorReadingsController"/>
    /// to connected Blazor clients.
    /// </summary>
    public class SensorHub : Hub
    {
        // This hub doesn't need to implement any methods for now,
        // it is just used for broadcasting messages to connected
        // clients.
    }
}
