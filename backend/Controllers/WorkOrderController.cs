using backend.DTOs;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
                //.Include(w => w.Car)
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

            // Security check - only allow viewing if admin, assigned mechanic, or client
            var currentUser = await _userManager.GetUserAsync(User);
            var userRoles = await _userManager.GetRolesAsync(currentUser);

            if (!userRoles.Contains("Admin") &&
                workOrder.MechanicId != currentUser.Id &&
                workOrder.ClientId != currentUser.Id)
            {
                return Forbid();
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

            return View(workOrderDetails);

        }

        // GET: WorkOrder/Edit/{id}

        [Authorize(Roles = "Admin,Mechanic")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workOrder = await _context.WorkOrders
                .Include(w => w.Car)
                .Include(w => w.WorkOrderServices)
                    .ThenInclude(wos => wos.Service)
                .FirstOrDefaultAsync(m => m.WorkOrderId == id);

            if (workOrder == null)
            {
                return NotFound();
            }

            // Security check for mechanics - can only edit their assigned work orders
            var currentUser = await _userManager.GetUserAsync(User);
            var userRoles = await _userManager.GetRolesAsync(currentUser);

            if (!userRoles.Contains("Admin") &&
                userRoles.Contains("Mechanic") &&
                workOrder.MechanicId != currentUser.Id)
            {
                return Forbid();
            }

            return View(workOrder);
        }

        // POST: WorkOrder/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Mechanic")]
        public async Task<IActionResult> Edit(int id, WorkOrder workOrder)
        {
            if (id != workOrder.WorkOrderId)
            {
                return NotFound();
            }

            // Security check for mechanics - can only edit their assigned work orders
            var currentUser = await _userManager.GetUserAsync(User);
            var userRoles = await _userManager.GetRolesAsync(currentUser);

            if (!userRoles.Contains("Admin") &&
                userRoles.Contains("Mechanic") &&
                workOrder.MechanicId != currentUser.Id)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workOrder);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkOrderExists(workOrder.WorkOrderId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(workOrder);
        }

        private bool WorkOrderExists(int id)
        {
            return _context.WorkOrders.Any(e => e.WorkOrderId == id);
        }

    }
}