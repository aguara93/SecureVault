using SecureVault.Shared.Enums;

namespace SecureVault.API.Models
{
    public class Sensor
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Description {  get; set; } = string.Empty;
        public SensorType Type { get; set; }
        public SensorStatus Status { get; set; }
        public string ApiKey { get; set; } = string.Empty; // displayed to the user once
        public string ApiKeyHash { get; set; } = string.Empty; // stored in the database
        public DateTime LastSeen { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public ICollection<SensorReading> Readings { get; set; } = new List<SensorReading>();
        public ICollection<AlarmEvent> AlarmEvents { get; set; } = new List<AlarmEvent>();
    }
}
