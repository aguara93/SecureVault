using Microsoft.EntityFrameworkCore;
using SecureVault.API.Models;

namespace SecureVault.API.Data
{
    public class SecureVaultDbContext : DbContext
    {
        public SecureVaultDbContext(DbContextOptions<SecureVaultDbContext> options) 
            : base(options)
        {
        }

        public DbSet<Sensor> Sensors { get; set; } = null!;
        public DbSet<SensorReading> SensorReadings { get; set; } = null!;
        public DbSet<AlarmEvent> AlarmEvents { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;

    }
}
