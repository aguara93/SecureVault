using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureVault.API.Data;
using SecureVault.API.Models;
using SecureVault.Shared.DTOs;

namespace SecureVault.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SensorReadingsController : ControllerBase
    {
        private readonly SecureVaultDbContext _context;

        public SensorReadingsController(SecureVaultDbContext context)
        {
            _context = context;
        }

        // GET: api/sensorreadings/sensor/5
        [HttpGet("sensor/{sensorId}")]
        public async Task<ActionResult<IEnumerable<SensorReadingDto>>> GetReadingsBySensor(int sensorId)
        {
            return await _context.SensorReadings
                .Where(r => r.SensorId == sensorId)
                .OrderByDescending(r => r.Timestamp)
                .Select(r => new SensorReadingDto
                {
                    Id = r.Id,
                    SensorId = r.SensorId,
                    Value = r.Value,
                    Unit = r.Unit,
                    Timestamp = r.Timestamp
                })
                .ToListAsync();
        }

        // POST: api/sensorreadings
        [HttpPost]
        public async Task<ActionResult<SensorReadingDto>> CreateReading(SensorReadingDto dto)
        {
            var reading = new SensorReading
            {
                SensorId = dto.SensorId,
                Value = dto.Value,
                Unit = dto.Unit,
                Timestamp = DateTime.UtcNow
            };

            _context.SensorReadings.Add(reading);
            await _context.SaveChangesAsync();

            dto.Id = reading.Id;
            return CreatedAtAction(nameof(GetReadingsBySensor),
                new { sensorId = reading.SensorId }, dto);
        }
    }
}