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

        /// <summary>
        /// Retrieves all alarm events, including both active and
        /// resolved alarms, together with a name and location of
        /// the sensor that triggered each one of them.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains a collection of AlarmEventDto
        /// objects.</returns>
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

        /// <summary>
        /// Retrieves only the alarm events that currently have the status 
        /// Triggered - alarms that have not yet been resolved.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains a collection of AlarmEventDto
        /// objects representing active alarms.</returns>
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

        /// <summary>
        /// Updates the status of an existing alarm event.
        /// If the new status is Resolved, the resolution timestamp
        /// is set automatically.
        /// </summary>
        /// <param name="id">The unique identifier of the alarm event
        /// to update.</param>
        /// <param name="status">The new status to assign to the 
        /// alarm event.</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains a 204 No Content response if the
        /// update succeeded or a 404 Not Found response otherwise.</returns>
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