using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Authorize(Roles = "User")]
    public class CustomerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }

}
