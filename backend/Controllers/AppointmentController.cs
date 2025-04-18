using System.Threading.Tasks;
using backend.DTOs;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AppointmentController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: https://localhost:7054/api/appointment/available?date=2025-04-18
        [HttpGet("available")]
        public async Task<ActionResult> GetAvailableSlots(DateTime date)
        {
            // check holidays
            bool isHoliday = await _context.Holidays.AnyAsync(h => h.Date.Date == date.Date);
            if (isHoliday)
                return Ok(new List<AvailableSlots>());

            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                return Ok(new List<AvailableSlots>());

            var slotLength = TimeSpan.FromHours(1);
            var standardSlots = new List<(TimeSpan Start, TimeSpan End)>
            {
                (TimeSpan.FromHours(8), TimeSpan.FromHours(12)),
                (TimeSpan.FromHours(13), TimeSpan.FromHours(16))
            };

            // get mechanics with availability
            var availableMechanicId = await _context.MechanicAvailabilities
                .Where(ma => ma.DayOfWeek == date.DayOfWeek)
                .Select(ma => ma.ApplicationUserId)
                .Distinct()
                .ToListAsync();

            if (!availableMechanicId.Any())
                return Ok(new List<AvailableSlots>());

            var availabilities = await _context.MechanicAvailabilities
                .Where(ma => ma.DayOfWeek == date.DayOfWeek && availableMechanicId.Contains(ma.ApplicationUserId))
                .ToListAsync();

            // Get booked slots
            var bookedSlots = await _context.WorkOrders
                .Where(w => w.AppointmentTime.Date == date.Date)
                .Select(w => w.AppointmentTime.TimeOfDay)
                .ToListAsync();

            var availableSlots = new List<AvailableSlots>();

            foreach (var block in standardSlots)
            {
                var current = block.Start;
                while (current + slotLength <= block.End)
                {
                    if (!bookedSlots.Contains(current))
                    {
                        var availableCount = availabilities
                            .Count(a => a.StartTime <= current && a.EndTime >= current + slotLength);

                        availableSlots.Add(new AvailableSlots
                        {
                            StartTime = current,
                            EndTime = current + slotLength,
                            AvailableMechanics = availableCount
                        });
                    }
                    current += slotLength;
                }
            }
            return Ok(availableSlots);


        }
        [HttpPost("book appointment")]
        [Authorize(Roles = "Client")]

        public async Task<IActionResult>BookAppointment(WorkOrder booking)
        {
            //TODO
            return null;
        }

    }
}


