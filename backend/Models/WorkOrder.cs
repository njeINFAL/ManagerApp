using System.ComponentModel.DataAnnotations.Schema;

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

        // User Id with two roles
        public string? ClientId { get; set; }
        [ForeignKey("ClientId")]
        public ApplicationUser? Client { get; set; }

        public string? MechanicId { get; set; }
        [ForeignKey("MechanicId")]
        public ApplicationUser? Mechanic { get; set; }

        public ICollection<WorkOrderService> WorkOrderServices { get; set; } = new List<WorkOrderService>();
    }
}
