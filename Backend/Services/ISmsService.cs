namespace Backend.Services
{
    public interface ISmsService
    {
        Task SendSmsAsync(string toPhoneNumber, string messageBody);
    }
}
