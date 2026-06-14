namespace SecureVault.API.Models
{
    public class SensorReading
    {
        public int Id { get; set; }
        public int SensorId { get; set; }
        public double Value { get; set; }
        public string Unit { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }

        // Navigation property
        public Sensor Sensor { get; set; } = null!;
    }
}
