using backend.DTOs;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AppointmentsApiController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: https://localhost:7054/api/appointment/available?date=2025-04-18
        [HttpGet("available")]
        public async Task<ActionResult> GetAvailableSlots(DateTime date)

            //Validation
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // check if selected date is not in the past
            if (date <= DateTime.Now.Date)
                return BadRequest("Nem lehet múltbeli időpontra foglalni");

            // check holidays
            bool isHoliday = await _context.Holidays.AnyAsync(h => h.Date.Date == date.Date);
            if (isHoliday)
                return BadRequest("Ünnepnapon nincs időpont");

            //check if date is not weekend
            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                return BadRequest("Hétvégén nincs időpont");

            //all slots are one hour long, working hours are fix between 8-12 and 13-16
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
                return BadRequest("Nincs elérhető szerelő erre az időpontra"); ;

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
            /*Sample response:
             [{"startTime":"08:00:00","endTime":"09:00:00","availableMechanics":1},{"startTime":"09:00:00",
            "endTime":"10:00:00","availableMechanics":1},{"startTime":"10:00:00","endTime":"11:00:00","availableMechanics":1},
            {"startTime":"11:00:00","endTime":"12:00:00","availableMechanics":1},{"startTime":"13:00:00","endTime":"14:00:00","availableMechanics":1},{"startTime":"14:00:00","endTime":"15:00:00","availableMechanics":1},{"startTime":"15:00:00","endTime":"16:00:00","availableMechanics":1}]
            */

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WorkOrderDetails>> GetWorkOrder(int id)
        {
            var workOrder = await _context.WorkOrders
                .Include(w => w.Client)
                .Include(w => w.Mechanic)
                .Include(w => w.WorkOrderServices)
                    .ThenInclude(ws => ws.Service)
                .FirstOrDefaultAsync(w => w.WorkOrderId == id);

            if (workOrder == null)
                return NotFound();

            var dto = new WorkOrderDetails
            {
                WorkOrderId = workOrder.WorkOrderId,
                AppointmentTime = workOrder.AppointmentTime,
                Notes = workOrder.Notes,
                IsActive = workOrder.IsActive,
                Car = workOrder.Car != null ? new CarDto
                {
                    LicencePlate = workOrder.Car.LicencePlate,
                    Manufacturer = workOrder.Car.Manufacturer,
                    Type = workOrder.Car.Type
                } : null,
                Client = workOrder.Client != null ? new UserDto
                {
                    FirstName = workOrder.Client.UserFirstNames,
                    LastName = workOrder.Client.UserLastName
                } : null,
                Mechanic = workOrder.Mechanic != null ? new UserDto
                {
                    FirstName = workOrder.Mechanic.UserFirstNames,
                    LastName = workOrder.Mechanic.UserLastName
                } : null,
                Services = workOrder.WorkOrderServices.Select(ws => new ServiceDto
                {
                    ServiceName = ws.Service.ServiceName,
                    ServiceDurationMinutes = ws.Service.ServiceDurationMinutes,
                    ServicePrice = ws.Service.ServicePrice,
                    Status = ws.Status.ToString()
                }).ToList()
            };

            return Ok(dto);
        }



        [HttpPost("book")]
        [Authorize(Roles = "Client")]

        public async Task<IActionResult>BookAppointment(AppointmentBookingRequest request)
        {
            //Validation
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Get current user ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            //Check if slots is not booked between GET and POST
            bool isBooked = await _context.WorkOrders
                .AnyAsync(w => w.AppointmentTime.Date == request.AppointmentTime.Date &&
                           w.AppointmentTime.TimeOfDay == request.AppointmentTime.TimeOfDay);
            if (isBooked)
                return BadRequest("Az időpont már foglalt!");

            // Verify at least one mechanic is available at this time
            var timeOfDay = request.AppointmentTime.TimeOfDay;
            var slotEnd = timeOfDay + TimeSpan.FromHours(1);
            var availableMechanicId = await _context.MechanicAvailabilities
                .Where(ma => ma.DayOfWeek == request.AppointmentTime.DayOfWeek &&
                           ma.StartTime <= timeOfDay &&
                           ma.EndTime >= slotEnd)
                .Select(ma => ma.ApplicationUserId)
                .ToListAsync();


            if (!availableMechanicId.Any())
                return BadRequest("Nincs elérhető szerelő erre az időpontra"); ;

            // Create the work order
            var workOrder = new WorkOrder
            {
                AppointmentTime = request.AppointmentTime,
                CreatedAt = DateTime.Now,
                IsActive = true,
                Notes = request.Notes,
                //CarId = request.CarId,
                ClientId = userId,
                MechanicId = availableMechanicId.First()
                // TODO: szerelő hozzárendelésen még finomítani kell :D
            };

            // Begin transaction
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Add the work order
                    _context.WorkOrders.Add(workOrder);
                    await _context.SaveChangesAsync();

                    // Add associated services if provided
                    if (request.ServiceIds?.Any() == true)
                    {
                        foreach (var serviceId in request.ServiceIds)
                        {
                            _context.WorkOrderServicess.Add(new WorkOrderService
                            {
                                WorkOrderId = workOrder.WorkOrderId,
                                ServiceId = serviceId
                            });
                        }
                        await _context.SaveChangesAsync();
                    }

                    await transaction.CommitAsync();

                    return CreatedAtAction(nameof(GetWorkOrder), new { id = workOrder.WorkOrderId }, new AppointmentResponse
                    {
                        WorkOrderId = workOrder.WorkOrderId,
                        AppointmentTime = workOrder.AppointmentTime
                    });

                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, "Hiba történt a foglalás feldolgozása közben");
                }
            }

        }

    }
}


