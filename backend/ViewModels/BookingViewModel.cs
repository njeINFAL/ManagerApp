using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace backend.ViewModels
{
    public class BookingViewModel
    {
        [Required]
        public DateTime? SelectedDate { get; set; }

        [Required]
        public string SelectedSlot { get; set; }

        public int? CarId { get; set; }

        [Required]
        public List<int> SelectedServiceIds { get; set; } = new();

        public string? Notes { get; set; }

        public List<SelectListItem> AvailableSlots { get; set; } = new();
    }
}
