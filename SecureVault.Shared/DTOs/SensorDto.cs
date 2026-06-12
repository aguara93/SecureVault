using SecureVault.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureVault.Shared.DTOs
{
    internal class SensorDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public SensorType Type { get; set; }
        public SensorStatus Status { get; set; }
        public DateTime LastSeen { get; set; }
    }
}
