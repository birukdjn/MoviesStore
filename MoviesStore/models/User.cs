

namespace MoviesStore.models
{
    
    public enum SubscriptionPlan
    {
        Basic,     
        Standard,  
        Premium     
    }

    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public SubscriptionPlan SubscriptionPlan { get; set; } = SubscriptionPlan.Basic;
        public ICollection<Profile> Profiles { get; set; } = [];
    }
}