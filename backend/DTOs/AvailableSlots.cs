namespace backend.DTOs
{
    public class AvailableSlots
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int AvailableMechanics { get; set; }
    }
}
