using System.ComponentModel.DataAnnotations;

namespace backend.DTOs
{
    public class AppointmentBookingRequest
    {
        [Required]
        public DateTime AppointmentTime { get; set; }
        public int WorkOrderId { get; set; }

        [Required]
        public int? CarId { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        // List of service IDs to associate with this work order
        public List<int>? ServiceIds { get; set; }
    }
}
