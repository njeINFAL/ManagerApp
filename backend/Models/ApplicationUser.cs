using Microsoft.AspNetCore.Identity;

namespace backend.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? UserFirstNames { get; set; }
        public string? UserLastName { get; set; }
        public string? UserCity { get; set; }
        public int? UserPostalCode { get; set; }
        public string? UserStreet { get; set; }
        public string? UserHouseNo { get; set; }

        public ICollection<Car> Cars { get; set; } = new List<Car>();
        public ICollection<WorkOrder> ClientWorkOrders { get; set; } = new List<WorkOrder>();
        public ICollection<WorkOrder> MechanicWorkOrders { get; set; } = new List<WorkOrder>();
        public ICollection<MechanicAvailability> MechanicAvailabilities { get; set; } = new List<MechanicAvailability>();
    }
}
