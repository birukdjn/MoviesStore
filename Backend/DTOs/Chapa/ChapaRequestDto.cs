namespace Backend.DTOs.Chapa
{
    public class ChapaRequestDto
    {
        public required string Amount { get; set; }
        public required string Currency { get; set; } = "ETB";
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string TxRef { get; set; }
        public required string CallbackUrl { get; set; }
    }
}
