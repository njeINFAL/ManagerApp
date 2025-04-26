namespace backend.Models
{
    public enum PartOrderStatus
    {
        Requested,
        Approved,
        Ordered,
        Arrived
    }
    public class PartOrder
    {
        public int PartOrderId { get; set; }
        public int workOrderId { get; set; }
        public string? Requestor { get; set; } = string.Empty;
        public ApplicationUser? Mechanic { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public string? Approver { get; set; }
        public PartOrderStatus Status { get; set; } = PartOrderStatus.Requested;

        public ICollection<PartOrderItem> PartOrderItems { get; set; } = new List<PartOrderItem>();
    }
}
