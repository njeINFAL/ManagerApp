using Microsoft.AspNetCore.Mvc;

namespace backend.ViewModels
{
    public class RegisterViewModel : Controller
    {
       
        
            public string Email { get; set; }
            public string Password { get; set; }
            public string Role { get; set; }
        

    }
}
