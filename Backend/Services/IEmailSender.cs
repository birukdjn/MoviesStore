using Backend.models;

namespace Backend.Services
{
    public interface IEmailSender
    {
        void SendEmail(Message message);
    }
}
