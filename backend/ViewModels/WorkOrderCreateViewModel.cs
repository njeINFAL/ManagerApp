using System.ComponentModel.DataAnnotations;

namespace backend.ViewModels
{
    public class WorkOrderCreateViewModel
    {
    public int WorkOrderId { get; set; }

    [Display(Name = "Időpont")]
    public DateTime AppointmentTime { get; set; }

    [Display(Name = "Megjegyzés")]
    public string? Notes { get; set; }

    public bool? IsActive { get; set; }
    }
}
