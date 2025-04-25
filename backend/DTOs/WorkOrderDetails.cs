using backend.Models;

namespace backend.DTOs
{
    public class WorkOrderDetails
    {
        public int WorkOrderId { get; set; }
        public DateTime AppointmentTime { get; set; }
        public string? Notes { get; set; }
        public bool? IsActive { get; set; }
        public CarDto? Car { get; set; }
        public UserDto? Client { get; set; }
        public UserDto? Mechanic { get; set; }
        public List<ServiceDto> Services { get; set; } = new();
    }

    public class CarDto
    {
        public int CarId { get; set; }
        public string? LicencePlate { get; set; }
        public string? Manufacturer { get; set; }
        public string? Type { get; set; }
        public string? VINnumber { get; set; }
        public string? EngineNumber { get; set; }
    }

    public class UserDto
    {
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? City { get; set; }
        public int? PostalCode { get; set; }
        public string? Street { get; set; }
        public string? HouseNo { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
    }

    public class ServiceDto
    {
        public int WorkOrderServiceId { get; set; }
        public int ServiceId { get; set; }
        public string? ServiceName { get; set; }
        public int ServiceDurationMinutes { get; set; }
        public int ServicePrice { get; set; }
        public string? Status { get; set; }
    }
}

