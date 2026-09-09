using Microsoft.AspNetCore.Mvc;
using SecureVault.API.Data;
using SecureVault.Shared.DTOs;
using SecureVault.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using SecureVault.API.Services;

namespace SecureVault.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SensorsController : ControllerBase
    {
        private readonly SecureVaultDbContext _context;
        private readonly ApiKeyService _apiKeyService;

        public SensorsController(SecureVaultDbContext context, ApiKeyService apiKeyService)
        {
            _context = context;
            _apiKeyService = apiKeyService;
        }

        // GET: api/sensors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SensorDto>>> GetSensors()
        {
            return await _context.Sensors
                .Select(s => new SensorDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Location = s.Location,
                    Type = s.Type,
                    Status = s.Status,
                    LastSeen = s.LastSeen
                })
                .ToListAsync();
        }

        // GET: api/sensors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SensorDto>> GetSensor(int id)
        {
            var sensor = await _context.Sensors.FindAsync(id);
            if (sensor == null) return NotFound();

            return new SensorDto
            {
                Id = sensor.Id,
                Name = sensor.Name,
                Location = sensor.Location,
                Type = sensor.Type,
                Status = sensor.Status,
                LastSeen = sensor.LastSeen
            };
        }

        // POST: api/sensors
        [HttpPost]
        public async Task<ActionResult<SensorDto>> CreateSensor(SensorDto sensorDto)
        {
            // Generate a new API key for this sensor
            var apiKey = _apiKeyService.GenerateApiKey();
            var apiKeyHash = _apiKeyService.HashApiKey(apiKey);

            var sensor = new Sensor
            {
                Name = sensorDto.Name,
                Location = sensorDto.Location,
                Type = sensorDto.Type,
                Status = sensorDto.Status,
                ApiKey = apiKey,   // shown once to the user
                ApiKeyHash = apiKeyHash,   // stored securely
                CreatedAt = DateTime.UtcNow,
                LastSeen = DateTime.UtcNow
            };

            _context.Sensors.Add(sensor);
            await _context.SaveChangesAsync();

            sensorDto.Id = sensor.Id;
            return CreatedAtAction(nameof(GetSensor), new { id = sensor.Id }, new 
            {
                sensor = sensorDto,
                apiKey = apiKey // return the raw key once, so the user can copy it
            });
        }

        // DELETE: api/sensors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSensor(int id)
        {
            var sensor = await _context.Sensors.FindAsync(id);
            if (sensor == null) return NotFound();

            _context.Sensors.Remove(sensor);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}