using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureVault.API.Data;
using SecureVault.API.Models;
using SecureVault.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using SecureVault.API.Services;
using SecureVault.Shared.Enums;

namespace SecureVault.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SensorReadingsController : ControllerBase
    {
        private readonly SecureVaultDbContext _context;
        private readonly AlarmEvaluationService _alarmService;

        public SensorReadingsController(SecureVaultDbContext context, AlarmEvaluationService alarmService)
        {
            _context = context;
            _alarmService = alarmService;
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
            // Find the sensor
            var sensor = await _context.Sensors.FindAsync(dto.SensorId);
            if (sensor == null)
            {
                return NotFound($"Sensor with ID {dto.SensorId} not found.");
            }

            // Save the reading
            var reading = new SensorReading
            {
                SensorId = dto.SensorId,
                Value = dto.Value,
                Unit = dto.Unit,
                Timestamp = DateTime.UtcNow
            };

            _context.SensorReadings.Add(reading);

            // Update sensor LastSeen
            sensor.LastSeen = DateTime.UtcNow;

            // Check if alarm needs to be triggered
            if (_alarmService.ShouldTriggerAlarm(sensor, dto.Value))
            {
                var alarm = new AlarmEvent
                {
                    SensorId = sensor.Id,
                    Status = AlarmStatus.Triggered,
                    Description = _alarmService.GetAlarmDescription(sensor, dto.Value),
                    TriggeredAt = DateTime.UtcNow
                };
                _context.AlarmEvents.Add(alarm);
            }

            await _context.SaveChangesAsync();

            dto.Id = reading.Id;
            return CreatedAtAction(nameof(GetReadingsBySensor),
                new { sensorId = reading.SensorId }, dto);
        }
    }
}