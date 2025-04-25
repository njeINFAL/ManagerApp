using backend.DTOs;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using backend.Models;
using backend.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;


namespace backend.Controllers
{
    public class BookingController : Controller
    {

        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Book()
        {

            var services = await _context.Services
                .Select( s => new SelectListItem
                {
                    Value = s.ServiceId.ToString(),
                    Text = $"{s.ServiceName} ({s.ServiceDurationMinutes} perc, {s.ServicePrice} Ft)"
                })
                .ToListAsync();


            var model = new BookingViewModel
            {
                //CarId = new int(),
                SelectedServiceIds = new List<int>(),
                AvailableSlots = new List<SelectListItem>(),
                AvailableServices = services
            };
            return View(model);
        }

            private async Task<(bool Success, string? ErrorMessage, int WorkOrderId, DateTime AppointmentTime)> BookAppointmentInternal(string userId, AppointmentBookingRequest request)

        {
            var timeOfDay = request.AppointmentTime.TimeOfDay;
            var slotEnd = timeOfDay + TimeSpan.FromHours(1);

            var isBooked = await _context.WorkOrders.AnyAsync(w =>
                w.AppointmentTime.Date == request.AppointmentTime.Date &&
                w.AppointmentTime.TimeOfDay == request.AppointmentTime.TimeOfDay);

            if (isBooked)
                return (false, "Az időpont már foglalt!",0,DateTime.Now);

            var availableMechanicIds = await _context.MechanicAvailabilities
                .Where(ma => ma.DayOfWeek == request.AppointmentTime.DayOfWeek &&
                             ma.StartTime <= timeOfDay &&
                             ma.EndTime >= slotEnd)
                .Select(ma => ma.ApplicationUserId)
                .ToListAsync();

            if (!availableMechanicIds.Any())
                return (false, "Nincs elérhető szerelő erre az időpontra",0,DateTime.Now);

            var workOrder = new WorkOrder
            {
                AppointmentTime = request.AppointmentTime,
                CreatedAt = DateTime.Now,
                IsActive = true,
                Notes = request.Notes,
                ClientId = userId,
                MechanicId = availableMechanicIds.First()
            };

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.WorkOrders.Add(workOrder);
                await _context.SaveChangesAsync();

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
                return (true, null, workOrder.WorkOrderId, workOrder.AppointmentTime);
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                var fullMessage = e.InnerException?.Message ?? e.Message;
                return (false, $"Hiba: {fullMessage}", 0,DateTime.Now);
            }
        }
        

        [HttpPost]
        public async Task<IActionResult> Book(BookingViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var appointmentTime = DateTime.Parse($"{model.SelectedDate:yyyy-MM-dd}T{model.SelectedSlot}");

            var request = new AppointmentBookingRequest
            {
                AppointmentTime = appointmentTime,
                CarId = model.CarId,
                Notes = model.Notes,
                ServiceIds = model.SelectedServiceIds
            };

            var result = await BookAppointmentInternal(userId, request);
            if (result.Success)
            {
                var response = new AppointmentResponse
                {
                    WorkOrderId = result.WorkOrderId,
                    AppointmentTime = result.AppointmentTime
                };

                return View("BookingResponse", response);

            }

            ViewBag.ErrorMessage = result.ErrorMessage ?? "Sikertelen foglalás!";
                return View("BookingResponse");
        }

    }
}
