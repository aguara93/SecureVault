using SecureVault.Shared.Enums;

namespace SecureVault.API.Models
{
    public class AlarmEvent
    {
        public int Id { get; set; }
        public int SensorId { get; set; }
        public AlarmStatus Status { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime TriggeredAt { get; set; }
        public DateTime? ResolvedAt { get; set; }

        // Navigation property
        public Sensor Sensor { get; set; } = null!;
    }
}
