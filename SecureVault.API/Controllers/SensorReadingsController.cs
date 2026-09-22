using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureVault.API.Data;
using SecureVault.API.Models;
using SecureVault.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using SecureVault.API.Services;
using SecureVault.Shared.Enums;
using Microsoft.AspNetCore.SignalR;
using SecureVault.API.Hubs;
using SecureVault.API.Attributes;

namespace SecureVault.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SensorReadingsController : ControllerBase
    {
        private readonly SecureVaultDbContext _context;
        private readonly AlarmEvaluationService _alarmService;
        private readonly IHubContext<SensorHub> _hubContext;

        public SensorReadingsController(
            SecureVaultDbContext context,
            AlarmEvaluationService alarmService,
            IHubContext<SensorHub> hubContext)
        {
            _context = context;
            _alarmService = alarmService;
            _hubContext = hubContext;
        }

        /// <summary>
        /// Retrieves sensor readings for the specified sensor, 
        /// ordered by timestamp in descending order 
        /// (from the most recent to the oldest).
        /// </summary>
        /// <param name="sensorId">The unique identifier of the sensor
        /// whose readings should be retrieved.</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains a collection of SensorReadingDto 
        /// objects belonging to the specified sensor.</returns>
        // GET: api/sensorreadings/sensor/5
        [HttpGet("sensor/{sensorId}")]
        [Authorize]  // Users still need JWT to view readings
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

        /// <summary>
        /// Receives a new sensor reading, persists it, evaluates whether
        /// it should trigger an alarm, and notifies all connected clients
        /// in real time via SignalR.
        /// </summary>
        /// <param name="dto">The data transfer object containing
        /// the sensor reading to record.</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains the newly created SensorReadingDto,
        /// or a 404 Not Found response if the sensor doesn´t exist.</returns>
        // POST: api/sensorreadings
        [HttpPost]
        [ApiKeyAuth]  // Sensors authenticate with their own API key instead of JWT
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

            // Notify all connected clients about the new reading
            await _hubContext.Clients.All.SendAsync("ReceiveSensorReading", dto);

            return CreatedAtAction(nameof(GetReadingsBySensor),
                new { sensorId = reading.SensorId }, dto);
        }
    }
}