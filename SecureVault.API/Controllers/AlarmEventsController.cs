using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureVault.API.Data;
using SecureVault.Shared.DTOs;
using SecureVault.Shared.Enums;
using Microsoft.AspNetCore.Authorization;

namespace SecureVault.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AlarmEventsController : ControllerBase
    {
        private readonly SecureVaultDbContext _context;

        public AlarmEventsController(SecureVaultDbContext context)
        {
            _context = context;
        }

        // GET: api/alarmevents
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AlarmEventDto>>> GetAlarmEvents()
        {
            return await _context.AlarmEvents
                .Include(a => a.Sensor)
                .Select(a => new AlarmEventDto
                {
                    Id = a.Id,
                    SensorId = a.SensorId,
                    SensorName = a.Sensor.Name,
                    SensorLocation = a.Sensor.Location,
                    Status = a.Status,
                    Description = a.Description,
                    TriggeredAt = a.TriggeredAt,
                    ResolvedAt = a.ResolvedAt
                })
                .ToListAsync();
        }

        // GET: api/alarmevents/active
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<AlarmEventDto>>> GetActiveAlarms()
        {
            return await _context.AlarmEvents
                .Include(a => a.Sensor)
                .Where(a => a.Status == AlarmStatus.Triggered)
                .Select(a => new AlarmEventDto
                {
                    Id = a.Id,
                    SensorId = a.SensorId,
                    SensorName = a.Sensor.Name,
                    SensorLocation = a.Sensor.Location,
                    Status = a.Status,
                    Description = a.Description,
                    TriggeredAt = a.TriggeredAt,
                    ResolvedAt = a.ResolvedAt
                })
                .ToListAsync();
        }

        // PUT: api/alarmevents/3/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateAlarmStatus(int id, AlarmStatus status)
        {
            var alarm = await _context.AlarmEvents.FindAsync(id);
            if (alarm == null) return NotFound();

            alarm.Status = status;
            if (status == AlarmStatus.Resolved)
                alarm.ResolvedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}