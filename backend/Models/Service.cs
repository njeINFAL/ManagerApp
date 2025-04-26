namespace backend.Models
{
    public class Service
    {
        public int ServiceId { get; set; }

        public string? ServiceName { get; set; }

        public int ServiceDurationMinutes { get; set; }

        public int ServicePrice { get; set; }

        public ICollection<WorkOrderService> WorkOrderServices { get; set; } = new List<WorkOrderService>();
    }
}
