namespace backend.Models
{
    public class PartItem
    {
        public int PartItemId { get; set; }
        public string? PartItemName { get; set; }
        public int PartsCategoryId { get; set; }

        public PartsCategory? Category { get; set; }

    }
}
