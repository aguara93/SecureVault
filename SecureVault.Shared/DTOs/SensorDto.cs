using SecureVault.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using SecureVault.Shared.Enums;

namespace SecureVault.Shared.DTOs
{
    /// <summary>
    /// Data transfer object representing a sensor, used when creating,
    /// retrieving, or displaying sensor information.
    /// </summary>
    public class SensorDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Sensor name is required.")]
        [MinLength(1, ErrorMessage = "Sensor name cannot be empty.")]
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public SensorType Type { get; set; }
        public SensorStatus Status { get; set; }
        public DateTime LastSeen { get; set; }
    }
}
