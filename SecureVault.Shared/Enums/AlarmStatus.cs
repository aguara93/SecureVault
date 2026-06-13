using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureVault.Shared.Enums
{
     public enum AlarmStatus
    {
            Active,
            Inactive,
            Triggered,
            Silenced,
            Acknowledged,
            Resolved,
            Reset
    }
}
