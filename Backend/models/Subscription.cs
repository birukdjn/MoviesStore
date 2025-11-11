using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.models
{
    public enum SubscriptionPlan
    {
        Basic,
        Standardsubs,
        Premium
    }

    public class Subscription
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        public SubscriptionPlan Plan { get; set; } = SubscriptionPlan.Basic;

        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; }
        public bool IsActive => EndDate == null || EndDate > DateTime.UtcNow;
    }
}
