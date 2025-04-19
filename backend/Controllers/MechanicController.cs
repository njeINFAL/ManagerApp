using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Authorize(Roles = "Mechanic")]
    public class MechanicController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }

}
