namespace backend.Models
{
    public class Car
    {
        public int CarId { get; set; }
        public string LicencePlate { get; set; } = string.Empty;
        public string? VINnumber { get; set; }
        public string? EngineNumber { get; set; }
        public string Manufacturer { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;

        // Külső kulcs és navigációs property a tulajdonoshoz
        public string? ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }

        // Navigációs property a munkalapokhoz
        public ICollection<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();
    }
}
