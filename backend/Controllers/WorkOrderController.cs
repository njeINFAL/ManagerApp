using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    //[Authorize]
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
            var workOrders = await _context.WorkOrders
                .Include(w => w.Car)
                .Include(w => w.Client)
                .Include(w => w.Mechanic)
                .Include(w => w.WorkOrderServices)
                    .ThenInclude(wos => wos.Service)
                .ToListAsync();

            return View(workOrders);
        }

        // GET: WorkOrder/Details/5
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

            return View(workOrder);
        }

        // POST: WorkOrder/ToggleStatus/5
        [HttpPost]
        [Authorize(Roles = "Admin,Mechanic")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var workOrder = await _context.WorkOrders.FindAsync(id);

            if (workOrder == null)
            {
                return Json(new { success = false, message = "Munkalap nem található." });
            }

            // Toggle status
            workOrder.IsActive = !workOrder.IsActive;

            try
            {
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}