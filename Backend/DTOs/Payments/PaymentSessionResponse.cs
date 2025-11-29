namespace Backend.DTOs.Payments
{
    public class PaymentSessionResponse
    {
        public string CheckoutUrl { get; set; } = string.Empty;
        public string TxRef { get; set; } = string.Empty;
    }
}
