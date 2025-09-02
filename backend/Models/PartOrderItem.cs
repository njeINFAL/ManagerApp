namespace backend.Models
{
    public class PartOrderItem
    {
        public int PartOrderItemId { get; set; }
        public int PartItemId { get; set; }
        public int PartOrderId { get; set; }
        public int Quantity {  get; set; }

        public PartItem? PartItem { get; set; }
        public PartOrder? PartOrder { get; set; }
    }
}
