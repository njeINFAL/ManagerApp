using backend.DTOs;
using backend.Models;
using backend.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [Authorize]
    public class WorkOrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public WorkOrderController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: WorkOrder
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var userRoles = await _userManager.GetRolesAsync(currentUser);

            var workOrdersQuery = _context.WorkOrders
                .Include(w => w.Car)
                .Include(w => w.Client)
                .Include(w => w.Mechanic)
                .Include(w => w.WorkOrderServices)
                    .ThenInclude(wos => wos.Service)
                .AsQueryable();

            if (userRoles.Contains("Client"))
            {
                workOrdersQuery = workOrdersQuery.Where(wo => wo.ClientId == currentUser.Id);
            }
            if (userRoles.Contains("Mechanic"))
            {
                workOrdersQuery = workOrdersQuery.Where(wo => wo.MechanicId == currentUser.Id);
            }

            var workOrders = await workOrdersQuery.ToListAsync();
            return View(workOrders);

        }

        // GET: WorkOrder/Details/{id}
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workOrder = await _context.WorkOrders
                .Include(w => w.Car)
                .Include(w => w.Client)
                .Include(w => w.Mechanic)
                .Include(w => w.WorkOrderServices)
                    .ThenInclude(wos => wos.Service)
                .FirstOrDefaultAsync(m => m.WorkOrderId == id);

            if (workOrder == null)
            {
                return NotFound();
            }

            // Mapping to WorkOrderDeatils DTO
            var workOrderDetails = new WorkOrderDetails
            {
                WorkOrderId = workOrder.WorkOrderId,
                AppointmentTime = workOrder.AppointmentTime,
                Notes = workOrder.Notes,
                IsActive = workOrder.IsActive,
                Car = workOrder.Car != null ? new CarDto
                {
                    LicencePlate = workOrder.Car.LicencePlate,
                    Manufacturer = workOrder.Car.Manufacturer,
                    Type = workOrder.Car.Type,
                    VINnumber = workOrder.Car.VINnumber,
                    EngineNumber = workOrder.Car.EngineNumber
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
                Services = workOrder.WorkOrderServices?.Select(wos => new ServiceDto
                {
                    ServiceName = wos.Service.ServiceName,
                    ServiceDurationMinutes = wos.Service.ServiceDurationMinutes,
                    ServicePrice = wos.Service.ServicePrice,
                    Status = wos.Status.ToString(),
                }).ToList() ?? new List<ServiceDto>()
            };

            var mechanicUsers = await _userManager.GetUsersInRoleAsync("Mechanic");
            ViewBag.Mechanics = mechanicUsers.Select(u => new SelectListItem
            {
                Value = u.Id,
                Text = u.UserLastName + " " + u.UserFirstNames
            }).ToList();

            return View(workOrderDetails);
        }

        // GET: WorkOrder/Cancel/{id}
        public async Task<IActionResult> Cancel(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workOrder = await _context.WorkOrders.FindAsync(id);
            if (workOrder == null)
            {
                return NotFound();
            }

            return View(workOrder);
        }


        // GET: WorkOrder/Create
        public IActionResult Create()
        {
            // TODO

            return View();
        }


        // POST:WorkOrder/Details/{id}

        [HttpPost]
        public async Task<IActionResult> UpdateAppointmentTime(int workOrderId, DateTime appointmentTime)
        {
            var order = await _context.WorkOrders.FindAsync(workOrderId);
            if (order == null) return NotFound();

            order.AppointmentTime = appointmentTime;
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = workOrderId });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateMechanic(int workOrderId, string mechanicId)
        {
            var order = await _context.WorkOrders.FindAsync(workOrderId);
            if (order == null) return NotFound();

            order.MechanicId = mechanicId;
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = workOrderId });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCarDetails(int workOrderId, string licencePlate, string manufacturer, string type, string VINnumber, string engineNumber)
        {
            var order = await _context.WorkOrders
                .Include(o => o.Car)
                .FirstOrDefaultAsync(o => o.WorkOrderId == workOrderId);

            if (order?.Car == null) return NotFound();

            order.Car.LicencePlate = licencePlate;
            order.Car.Manufacturer = manufacturer;
            order.Car.Type = type;
            order.Car.VINnumber = VINnumber;
            order.Car.EngineNumber = engineNumber;

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = workOrderId });
        }
        [HttpPost]
        public async Task<IActionResult> UpdateServiceStatus(int workOrderServiceId, string newStatus)
        {
            var wos = await _context.WorkOrderServicess.FindAsync(workOrderServiceId);
            if (wos == null) return NotFound();

            if (!Enum.TryParse<WorkOrderServiceStatus>(newStatus, out var parsed))
                return BadRequest();

            wos.Status = parsed;
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = wos.WorkOrderId });
        }
        [HttpPost]
        public async Task<IActionResult> Complete(int id)
        {
            var workOrder = await _context.WorkOrders.FindAsync(id);
            if (workOrder == null)
            {
                return NotFound();
            }

            workOrder.IsActive = false;
            workOrder.Notes = (workOrder.Notes ?? "") + " [TELJESÍTVE]";
            _context.Update(workOrder);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // POST: WorkOrder/Cancel/{id}
        [HttpPost, ActionName("Cancel")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelConfirmed(int id)
        {
            var workOrder = await _context.WorkOrders.FindAsync(id);
            workOrder.IsActive = false;
            workOrder.Notes = (workOrder.Notes ?? "") + " [TÖRÖLVE]";
            _context.Update(workOrder);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool WorkOrderExists(int id)
        {
            return _context.WorkOrders.Any(e => e.WorkOrderId == id);
        }

    }
}