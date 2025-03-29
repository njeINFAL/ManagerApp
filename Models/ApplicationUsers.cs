using Microsoft.AspNetCore.Identity;

namespace ManagerApp.Models
{
    public class ApplicationUsers : IdentityUser

    {
        public string? UserFirstNames { get; set; }
        public string? UserLastName { get; set; }
        public string? UserCity { get; set; }
        public int? UserPostalCode { get; set; }
        public string? UserSteet { get; set; }
        public string? UserHouseNo { get; set; }

    }
}
