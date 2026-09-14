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

        /// <summary>
        /// Retrieves all sensors as data transfer objects.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. 
        /// The task result contains a collection of SensorDto
        /// objects.</returns>
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

        /// <summary>
        ///  Retrives a single sensor identified by its unique ID.
        /// </summary>
        /// <param name="id">The unique identifier of the sensor to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains the requested SensorSto
        /// if found, or a 404 Not Found response otherwise.</returns>
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

        /// <summary>
        /// Creates a new sensor and generates a unique API key for it.
        /// The raw API key is returned only in thi response and cannot
        /// be retrieved again; only its hashed form is persisted.
        /// </summary>
        /// <param name="sensorDto">The data transfer object containing
        /// the datails of the sensor to create.</param>
        /// <returns>A task result contains the newly created sensor together
        /// with its generated API key.</returns>
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

        /// <summary>
        /// Deletes the sensor identified by its unique ID.
        /// </summary>
        /// <param name="id">The unique identifier of the sensor to delete.</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains a 204 No Content response if the
        /// delation succeeded, or a 404 Not Found response otherwise.</returns>
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