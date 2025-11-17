namespace Backend.models
{
    
   
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? Avatar { get; set; }
        public string Role { get; set; } = "User";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? LastLoginIp { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }

        public bool IsSubscribed { get; set; } = false;
        public DateTime? SubscriptionExpiresAt { get; set; }




        public ICollection<Subscription> Subscriptions { get; set; } = [];
        public ICollection<Profile> Profiles { get; set; } = [];

    }
}