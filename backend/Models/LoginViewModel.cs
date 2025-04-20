using Microsoft.AspNetCore.Mvc;

namespace backend.Models
{
    public class LoginViewModel : Controller
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
