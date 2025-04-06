namespace backend.Models
{
    public class WorkOrder
    {
        public int WorkOrderId { get; set; }

        public DateTime AppointmentTime { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool? IsActive { get; set; }

        public string? Notes { get; set; }  

        // Külső kulcs és navigációs property az autóhoz
        public int? CarId { get; set; }
        public Car? Car { get; set; }

        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public ICollection<WorkOrderService> WorkOrderServices { get; set; } = new List<WorkOrderService>();
    }
}
