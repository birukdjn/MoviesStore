namespace Backend.DTOs.Payments
{
    public class SubscriptionDto
    {
        public int Id { get; set; }
        public string Plan { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
