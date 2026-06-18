using SecureVault.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureVault.Shared.DTOs
{
    public class AlarmEventDto
    {
        public int Id { get; set; }
        public int SensorId { get; set; }
        public string SensorName { get; set; } = string.Empty;
        public string SensorLocation { get; set; } = string.Empty;
        public SensorType SensorType { get; set; }
        public AlarmStatus Status { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime TriggeredAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }
}
