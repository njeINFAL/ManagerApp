namespace backend.Models
{
    public class PartsCategory
    {
        public int PartsCategoryId { get; set; }
        public string? PartsCategoryName { get; set; }

        public ICollection<PartItem> PartItems { get; set; } = new List<PartItem>();
    }
}
