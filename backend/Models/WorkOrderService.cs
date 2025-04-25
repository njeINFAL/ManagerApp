namespace backend.Models
{
    public enum WorkOrderServiceStatus
    {
        Pending,      
        InProgress,
        Completed,
        Cancelled
    }
    public class WorkOrderService
    {
        public int WorkOrderServiceID { get; set; }
        public int WorkOrderId { get; set; }
        public WorkOrder? WorkOrder { get; set; }
        public int ServiceId { get; set; }
        public Service? Service { get; set; }
        public string? ResponsibleUserId { get; set; }
        public ApplicationUser? ResponsibleUser { get; set; }
        public WorkOrderServiceStatus Status { get; set; } = WorkOrderServiceStatus.Pending;

    }
}
