namespace backend.Models
{
    public class WorkOrder
    {
        public int WorkOrderId { get; set; }

        public DateTime AppointmentTime { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool? IsActive { get; set; }

        public string? Notes { get; set; }  // most már nullable, ha üres string nem elég

        // Külső kulcs és navigációs property az autóhoz
        public int? CarId { get; set; }
        public Car? Car { get; set; }
    }
}
